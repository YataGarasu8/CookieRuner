using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("BGM Settings")]
    public AudioSource bgmSource;                 // 배경음악 재생용 AudioSource
    [Range(0f, 1f)] public float bgmVolume = 0.5f; // BGM 볼륨 설정

    [Header("SFX Settings")]
    public int sfxPoolSize = 10;                  // 동시에 재생할 수 있는 최대 SFX 채널 수
    [Range(0f, 1f)] public float sfxVolume = 0.7f; // SFX 볼륨 설정
    public List<AudioClip> sfxClips;             // 인스펙터에서 추가할 SFX 클립 목록

    private Dictionary<string, AudioClip> sfxDictionary = new Dictionary<string, AudioClip>();
    private Queue<AudioSource> sfxPool = new Queue<AudioSource>(); // SFX용 AudioSource 풀

    [Header("BGM Clips")]
    public List<AudioClip> bgmClips;              // 인스펙터에서 추가할 BGM 클립 목록
    private Dictionary<string, AudioClip> bgmDictionary = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 중복 방지
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환 시 유지

        InitializeAudioDictionaries();
        InitializeSFXPool();
        ApplyVolumeSettings();
    }

    // 모든 오디오 클립을 사전(Dictionary)에 등록
    private void InitializeAudioDictionaries()
    {
        foreach (var clip in bgmClips)
            bgmDictionary[clip.name] = clip;

        foreach (var clip in sfxClips)
            sfxDictionary[clip.name] = clip;
    }

    // SFX 재생을 위한 AudioSource 풀 초기화
    private void InitializeSFXPool()
    {
        for (int i = 0; i < sfxPoolSize; i++)
        {
            GameObject sfxObject = new GameObject($"SFXSource_{i}");
            sfxObject.transform.SetParent(transform);
            AudioSource source = sfxObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.volume = sfxVolume;
            sfxPool.Enqueue(source); // 풀에 추가
        }
    }

    // 볼륨 설정 적용
    private void ApplyVolumeSettings()
    {
        bgmSource.volume = bgmVolume;
        foreach (var source in sfxPool)
            source.volume = sfxVolume;
    }

    // BGM 재생
    public void PlayBGM(string name, bool loop = true)
    {
        if (bgmDictionary.TryGetValue(name, out var clip))
        {
            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning($"BGM '{name}'을(를) 찾을 수 없습니다.");
        }
    }

    // BGM 중지
    public void StopBGM() => bgmSource.Stop();

    // SFX 재생 (풀링 사용)
    public void PlaySFX(string name)
    {
        if (!sfxDictionary.TryGetValue(name, out var clip))
        {
            Debug.LogWarning($"SFX '{name}'을(를) 찾을 수 없습니다.");
            return;
        }

        AudioSource source = GetAvailableAudioSource();
        source.clip = clip;
        source.Play();
        StartCoroutine(ReturnToPoolAfterPlayback(source, clip.length));
    }

    // 사용 가능한 AudioSource 가져오기 (큐 사용)
    private AudioSource GetAvailableAudioSource()
    {
        AudioSource source = sfxPool.Dequeue(); // 큐에서 꺼냄
        sfxPool.Enqueue(source);                // 재사용 위해 다시 큐에 추가
        return source;
    }

    // 재생 완료 후 AudioSource 초기화
    private System.Collections.IEnumerator ReturnToPoolAfterPlayback(AudioSource source, float duration)
    {
        yield return new WaitForSeconds(duration);
        source.clip = null; // 메모리 최적화를 위해 클립 해제
    }

    // BGM 볼륨 설정
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        bgmSource.volume = bgmVolume;
    }

    // SFX 볼륨 설정
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        foreach (var source in sfxPool)
            source.volume = sfxVolume;
    }

    // 모든 오디오 음소거 전환
    public void MuteAll(bool mute)
    {
        bgmSource.mute = mute;
        foreach (var source in sfxPool)
            source.mute = mute;
    }

    // BGM 음소거 토글
    public void ToggleMuteBGM() => bgmSource.mute = !bgmSource.mute;

    // SFX 음소거 토글
    public void ToggleMuteSFX()
    {
        foreach (var source in sfxPool)
            source.mute = !source.mute;
    }
}
