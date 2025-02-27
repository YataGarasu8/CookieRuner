using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using UnityEngine.Networking;


// MonoBehaviour를 상속하지 않으나, 내부적으로 SoundManagerBehaviour를 통해 Unity 기능(transform, StartCoroutine 등)을 사용할 수 있음
// BGM 및 SFX 재생, 볼륨 조절, 사운드 로드 및 풀링 기능을 제공
public class SoundManager
{
    // 싱글톤 인스턴스를 저장하는 정적 변수
    private static SoundManager instance;

    // 외부에서 접근 가능한 싱글톤 인스턴스 프로퍼티
    public static SoundManager Instance
    {
        get
        {
            // 인스턴스가 존재하지 않으면 생성 및 초기화
            if (instance == null)
            {
                instance = new SoundManager();
                instance.Init(); // 초기화 함수 호출
            }
            return instance; // 인스턴스 반환
        }
    }

    // BGM 및 SFX 설정 관련 변수
    public AudioSource bgmSource;                 // BGM 재생용 AudioSource
    public float bgmVolume = 0.5f;                // BGM 볼륨 (0 ~ 1)
    public int sfxPoolSize = 10;                  // 동시에 재생 가능한 SFX 채널 수
    public float sfxVolume = 0.7f;                // SFX 볼륨 (0 ~ 1)

    // 사운드 파일을 저장하는 컬렉션
    private Dictionary<string, AudioClip> sfxDictionary = new Dictionary<string, AudioClip>(); // SFX 이름-클립 매핑
    private Dictionary<string, AudioClip> bgmDictionary = new Dictionary<string, AudioClip>(); // BGM 이름-클립 매핑

    // SFX 재생을 위한 AudioSource 풀
    private Queue<AudioSource> sfxPool = new Queue<AudioSource>();

    // 초기화 함수: 인스턴스 생성 시 한 번만 호출
    private void Init()
    {
        // GameObject 생성 및 SoundManagerBehaviour 추가 (코루틴과 transform 사용을 위해)
        var soundManagerObject = new GameObject("SoundManager");
        UnityEngine.Object.DontDestroyOnLoad(soundManagerObject); // 씬 전환 시 유지
        soundManagerObject.AddComponent<SoundManagerBehaviour>(); // MonoBehaviour 기능 제공

        // BGM용 AudioSource 초기화 및 설정
        bgmSource = soundManagerObject.AddComponent<AudioSource>();
        bgmSource.loop = true; // 기본적으로 반복 재생 설정
        bgmSource.volume = bgmVolume; // 초기 BGM 볼륨 적용

        // SFX 재생을 위한 AudioSource 풀 초기화
        InitializeSFXPool();

        // 사운드 파일 로드
        LoadSounds();

        // 초기 볼륨 적용
        ApplyVolumeSettings();
    }



    // 지정된 폴더에서 오디오 파일을 로드하여 사전에 등록
    public void LoadSounds(string folderPath = @"00. Art/11. Sound")
    {
        // 프로젝트 루트에서 절대 경로 생성 (Application.dataPath는 'Assets' 경로 반환)
        string soundFolderPath = Path.Combine(Application.dataPath, folderPath);

        // 폴더 존재 여부 확인
        if (!Directory.Exists(soundFolderPath))
        {
            Debug.LogError($"폴더 {soundFolderPath}가 존재하지 않습니다.");
            return;
        }

        // 지원되는 오디오 파일 확장자 목록
        string[] supportedExtensions = { ".mp3", ".wav", ".ogg" };

        // 모든 오디오 파일 검색
        foreach (var file in Directory.GetFiles(soundFolderPath, "*.*", SearchOption.AllDirectories))
        {
            string extension = Path.GetExtension(file).ToLower();

            // 지원하지 않는 파일은 건너뜀
            if (!supportedExtensions.Contains(extension))
                continue;

            string fileName = Path.GetFileNameWithoutExtension(file); // 파일명(확장자 제거)
            string fileUri = $"file://{file.Replace("\\", "/")}";     // 경로 구분자 변환 및 URI 형식

            // 코루틴으로 비동기 오디오 로드
            SoundManagerBehaviour.Instance.RunCoroutine(LoadAudioClip(fileUri, fileName));
        }
    }

    // 오디오 파일을 비동기로 로드하는 코루틴
    private System.Collections.IEnumerator LoadAudioClip(string fileUri, string clipName)
    {
        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(fileUri, AudioType.UNKNOWN))
        {
            yield return request.SendWebRequest(); // 요청 대기

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"오디오 로드 실패: {clipName} - {request.error}");
                yield break;
            }

            // AudioClip 생성 및 등록
            AudioClip clip = DownloadHandlerAudioClip.GetContent(request);

            // 경로 기반 등록 (bgm 폴더 포함 시 BGM 등록)
            if (fileUri.ToLower().Contains("/bgm/") || clipName.ToLower().Contains("bgm"))
            {
                bgmDictionary[clipName] = clip;
                Debug.Log($"BGM 로드 완료: {clipName}");
            }
            else
            {
                sfxDictionary[clipName] = clip;
                Debug.Log($"SFX 로드 완료: {clipName}");
            }
        }
    }



    // SFX 재생을 위한 AudioSource 풀 초기화
    private void InitializeSFXPool()
    {
        for (int i = 0; i < sfxPoolSize; i++)
        {
            // 새로운 GameObject 생성 및 AudioSource 추가
            GameObject sfxObject = new GameObject($"SFXSource_{i}");
            sfxObject.transform.SetParent(SoundManagerBehaviour.Instance.transform); // Helper 오브젝트에 부착

            // AudioSource 설정
            AudioSource source = sfxObject.AddComponent<AudioSource>();
            source.playOnAwake = false; // 자동 재생 비활성화
            source.volume = sfxVolume;  // 초기 SFX 볼륨 적용

            // 풀에 AudioSource 추가
            sfxPool.Enqueue(source);
        }
    }

    // 모든 AudioSource에 볼륨 적용
    private void ApplyVolumeSettings()
    {
        bgmSource.volume = bgmVolume; // BGM 볼륨 적용

        // 모든 SFX AudioSource에 볼륨 적용
        foreach (var source in sfxPool)
        {
            source.volume = sfxVolume;
        }
    }

    // 지정한 이름의 BGM 재생
    public void PlayBGM(string name, bool loop = true)
    {
        // BGM 클립 검색
        if (bgmDictionary.TryGetValue(name, out var clip))
        {
            bgmSource.clip = clip;   // AudioSource에 클립 할당
            bgmSource.loop = loop;   // 반복 재생 여부 설정
            bgmSource.Play();        // BGM 재생 시작
        }
        else
        {
            Debug.LogWarning($"BGM '{name}'을(를) 찾을 수 없습니다."); // 해당 이름의 클립이 없을 때 경고 출력
        }
    }

    // 현재 재생 중인 BGM 중지
    public void StopBGM()
    {
        bgmSource.Stop(); // BGM 재생 중지
    }

    // 지정한 이름의 SFX 재생
    public void PlaySFX(string name)
    {
        // SFX 클립 검색
        if (!sfxDictionary.TryGetValue(name, out var clip))
        {
            Debug.LogWarning($"SFX '{name}'을(를) 찾을 수 없습니다."); // 클립 미발견 시 경고 출력
            return;
        }

        // 사용 가능한 AudioSource 가져오기
        AudioSource source = GetAvailableAudioSource();
        source.clip = clip; // 클립 할당
        source.Play();      // 효과음 재생

        // 코루틴 실행을 위해 Helper 클래스를 사용
        SoundManagerBehaviour.Instance.RunCoroutine(ReturnToPoolAfterPlayback(source, clip.length));
    }

    // 사용 가능한 AudioSource 가져오기 (모두 사용 중일 경우 큐의 첫 번째 사용)
    private AudioSource GetAvailableAudioSource()
    {
        AudioSource source = sfxPool.Dequeue(); // 큐에서 AudioSource 가져오기
        sfxPool.Enqueue(source);                // 재사용을 위해 다시 큐에 추가
        return source;                          // 가져온 AudioSource 반환
    }

    // 효과음 재생 후 일정 시간 경과 시 클립 초기화 (메모리 최적화)
    private System.Collections.IEnumerator ReturnToPoolAfterPlayback(AudioSource source, float duration)
    {
        yield return new WaitForSeconds(duration); // 지정된 시간 동안 대기
        source.clip = null;                        // 클립 해제
    }

    // BGM 볼륨 조절
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume); // 0 ~ 1 범위 제한
        bgmSource.volume = bgmVolume;      // 볼륨 적용
    }

    // SFX 볼륨 조절
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume); // 0 ~ 1 범위 제한
        foreach (var source in sfxPool)
        {
            source.volume = sfxVolume; // 모든 SFX AudioSource에 적용
        }
    }

    // 전체 오디오 음소거 및 해제
    public void MuteAll(bool mute)
    {
        bgmSource.mute = mute; // BGM 음소거 적용
        foreach (var source in sfxPool)
        {
            source.mute = mute; // 모든 SFX 음소거 적용
        }
    }

    // BGM 음소거 토글
    public void ToggleMuteBGM()
    {
        bgmSource.mute = !bgmSource.mute; // 현재 상태 반전
    }

    // SFX 음소거 토글
    public void ToggleMuteSFX()
    {
        foreach (var source in sfxPool)
        {
            source.mute = !source.mute; // 현재 상태 반전
        }
    }
}