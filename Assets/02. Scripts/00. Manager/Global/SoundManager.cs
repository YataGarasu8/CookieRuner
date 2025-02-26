using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance     // 싱글톤 인스턴스
    {
        get
        {
            if (instance == null)
            {
                instance = new SoundManager();
                instance.Init(); // 인스턴스 생성 시 초기화 함수 호출
            }
            return instance;
        }
    }

    [Header("BGM Settings")]
    public AudioSource bgmSource;                 // 배경음악(BGM)을 재생하는 AudioSource
    [Range(0f, 1f)] public float bgmVolume = 0.5f; // BGM 볼륨 (0 ~ 1 사이 설정 가능)

    [Header("SFX Settings")]
    public int sfxPoolSize = 10;                  // 동시에 재생할 수 있는 최대 SFX 채널 수
    [Range(0f, 1f)] public float sfxVolume = 0.7f; // 효과음(SFX) 볼륨
    public List<AudioClip> sfxClips;              // 인스펙터에서 추가할 수 있는 SFX 오디오 클립 목록

    private Dictionary<string, AudioClip> sfxDictionary = new Dictionary<string, AudioClip>(); // SFX 클립을 이름으로 찾기 위한 사전
    private Queue<AudioSource> sfxPool = new Queue<AudioSource>(); // SFX 재생을 위한 AudioSource 풀

    [Header("BGM Clips")]
    public List<AudioClip> bgmClips;              // 인스펙터에서 추가할 수 있는 BGM 오디오 클립 목록
    private Dictionary<string, AudioClip> bgmDictionary = new Dictionary<string, AudioClip>(); // BGM 클립을 이름으로 찾기 위한 사전

    // 초기화 함수: 인스턴스 생성 시 필요한 초기 설정 수행
    private void Init()
    {
        // GameObject 생성 및 설정
        var soundManagerObject = new GameObject("SoundManager");
        UnityEngine.Object.DontDestroyOnLoad(soundManagerObject); // 씬 전환 시에도 유지

        // BGM용 AudioSource 추가 및 초기 설정
        bgmSource = soundManagerObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.volume = bgmVolume;

        // SFX 풀 초기화
        InitializeSFXPool();

        // 사운드 리소스 로드
        LoadSounds();

        // 볼륨 설정 적용
        ApplyVolumeSettings();
    }

    // 지정된 폴더에서 모든 오디오 파일을 로드하고 BGM/SFX 사전에 등록
    public void LoadSounds(string folderPath = "Sound")
    {
        // Resources 폴더 하위의 경로를 지정
        string soundFolderPath = Path.Combine(Application.dataPath, "Resources", folderPath);

        // 폴더가 존재하지 않으면 오류 로그 출력 후 함수 종료
        if (!Directory.Exists(soundFolderPath))
        {
            Debug.LogError($"폴더 {soundFolderPath}가 존재하지 않습니다.");
            return;
        }

        // 해당 폴더 및 모든 하위 폴더의 오디오 파일 검색
        foreach (var file in Directory.GetFiles(soundFolderPath, "*.*", SearchOption.AllDirectories))
        {
            // 파일 확장자를 소문자로 변환하여 오디오 파일 여부 확인
            string extension = Path.GetExtension(file).ToLower();

            // 지원하는 오디오 포맷만 처리
            if (extension == ".mp3" || extension == ".wav" || extension == ".ogg")
            {
                // Resources.Load를 위한 상대 경로 구성 (확장자 제거 및 경로 구분자 수정)
                string relativePath = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(file)).Replace("\\", "/");

                // Resources.Load를 통해 AudioClip 로드
                AudioClip clip = Resources.Load<AudioClip>(relativePath);

                if (clip != null)
                {
                    string clipName = clip.name; // 오디오 클립 이름으로 키 설정

                    // 파일 이름에 "bgm" 포함 시 BGM 사전에 추가, 그렇지 않으면 SFX 사전에 추가
                    if (clipName.ToLower().Contains("bgm"))
                    {
                        bgmDictionary[clipName] = clip;
                        Debug.Log($"BGM 로드: {clipName}");
                    }
                    else
                    {
                        sfxDictionary[clipName] = clip;
                        Debug.Log($"SFX 로드: {clipName}");
                    }
                }
                else
                {
                    Debug.LogWarning($"사운드 파일을 로드하지 못했습니다: {relativePath}");
                }
            }
        }
    }

    // SFX 재생을 위해 AudioSource 객체들을 풀링 방식으로 초기화
    private void InitializeSFXPool()
    {
        for (int i = 0; i < sfxPoolSize; i++)
        {
            // AudioSource를 담을 새로운 GameObject 생성
            GameObject sfxObject = new GameObject($"SFXSource_{i}");
            sfxObject.transform.SetParent(transform); // SoundManager 오브젝트의 자식으로 추가

            // AudioSource 컴포넌트 추가 및 설정
            AudioSource source = sfxObject.AddComponent<AudioSource>();
            source.playOnAwake = false; // 자동 재생 방지
            source.volume = sfxVolume;  // 초기 볼륨 설정

            // AudioSource를 큐에 추가 (풀링)
            sfxPool.Enqueue(source);
        }
    }

    // 초기 볼륨 설정을 적용 (BGM 및 SFX)
    private void ApplyVolumeSettings()
    {
        bgmSource.volume = bgmVolume; // BGM 볼륨 설정

        // 모든 SFX AudioSource에 볼륨 적용
        foreach (var source in sfxPool)
            source.volume = sfxVolume;
    }

    // 지정한 이름의 BGM을 재생
    public void PlayBGM(string name, bool loop = true)
    {
        // BGM 사전에서 해당 이름의 클립 검색
        if (bgmDictionary.TryGetValue(name, out var clip))
        {
            bgmSource.clip = clip;   // BGM 소스에 클립 할당
            bgmSource.loop = loop;   // 반복 재생 여부 설정
            bgmSource.Play();        // BGM 재생
        }
        else
        {
            Debug.LogWarning($"BGM '{name}'을(를) 찾을 수 없습니다."); // 클립 미발견 시 경고 출력
        }
    }

    // 현재 재생 중인 BGM을 중지
    public void StopBGM() => bgmSource.Stop();

    // 지정한 이름의 SFX를 재생 (풀링 사용)
    public void PlaySFX(string name)
    {
        // SFX 사전에서 해당 이름의 클립 검색
        if (!sfxDictionary.TryGetValue(name, out var clip))
        {
            Debug.LogWarning($"SFX '{name}'을(를) 찾을 수 없습니다."); // 클립 미발견 시 경고 출력
            return;
        }

        // 사용 가능한 AudioSource 가져오기
        AudioSource source = GetAvailableAudioSource();
        source.clip = clip; // 오디오 클립 할당
        source.Play();      // 효과음 재생

        // 재생 완료 후 AudioSource 초기화를 위해 코루틴 실행
        StartCoroutine(ReturnToPoolAfterPlayback(source, clip.length));
    }

    // 사용 가능한 AudioSource를 큐에서 가져오기
    private AudioSource GetAvailableAudioSource()
    {
        AudioSource source = sfxPool.Dequeue(); // 큐에서 AudioSource 꺼내기
        sfxPool.Enqueue(source);                // 재사용을 위해 다시 큐에 추가
        return source;
    }

    // SFX 재생 후 일정 시간이 지나면 AudioSource 초기화
    private System.Collections.IEnumerator ReturnToPoolAfterPlayback(AudioSource source, float duration)
    {
        yield return new WaitForSeconds(duration); // 클립 길이만큼 대기
        source.clip = null;                        // 메모리 최적화를 위해 클립 해제
    }

    // BGM 볼륨 설정
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume); // 볼륨 값을 0 ~ 1 사이로 제한
        bgmSource.volume = bgmVolume;      // BGM 소스에 적용
    }

    // SFX 볼륨 설정
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume); // 볼륨 값을 0 ~ 1 사이로 제한
        foreach (var source in sfxPool)
            source.volume = sfxVolume;     // 모든 SFX AudioSource에 적용
    }

    // 모든 오디오 음소거 또는 해제
    public void MuteAll(bool mute)
    {
        bgmSource.mute = mute; // BGM 음소거 설정
        foreach (var source in sfxPool)
            source.mute = mute; // 모든 SFX 음소거 설정
    }

    // BGM 음소거 토글
    public void ToggleMuteBGM() => bgmSource.mute = !bgmSource.mute;

    // SFX 음소거 토글
    public void ToggleMuteSFX()
    {
        foreach (var source in sfxPool)
            source.mute = !source.mute; // 각 SFX AudioSource의 음소거 상태 토글
    }
}
