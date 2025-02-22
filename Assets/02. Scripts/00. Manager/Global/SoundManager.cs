using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("BGM Settings")]
    public AudioSource bgmSource;                 // ������� ����� AudioSource
    [Range(0f, 1f)] public float bgmVolume = 0.5f; // BGM ���� ����

    [Header("SFX Settings")]
    public int sfxPoolSize = 10;                  // ���ÿ� ����� �� �ִ� �ִ� SFX ä�� ��
    [Range(0f, 1f)] public float sfxVolume = 0.7f; // SFX ���� ����
    public List<AudioClip> sfxClips;             // �ν����Ϳ��� �߰��� SFX Ŭ�� ���

    private Dictionary<string, AudioClip> sfxDictionary = new Dictionary<string, AudioClip>();
    private Queue<AudioSource> sfxPool = new Queue<AudioSource>(); // SFX�� AudioSource Ǯ

    [Header("BGM Clips")]
    public List<AudioClip> bgmClips;              // �ν����Ϳ��� �߰��� BGM Ŭ�� ���
    private Dictionary<string, AudioClip> bgmDictionary = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // �ߺ� ����
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // �� ��ȯ �� ����

        InitializeAudioDictionaries();
        InitializeSFXPool();
        ApplyVolumeSettings();
    }

    // ��� ����� Ŭ���� ����(Dictionary)�� ���
    private void InitializeAudioDictionaries()
    {
        foreach (var clip in bgmClips)
            bgmDictionary[clip.name] = clip;

        foreach (var clip in sfxClips)
            sfxDictionary[clip.name] = clip;
    }

    // SFX ����� ���� AudioSource Ǯ �ʱ�ȭ
    private void InitializeSFXPool()
    {
        for (int i = 0; i < sfxPoolSize; i++)
        {
            GameObject sfxObject = new GameObject($"SFXSource_{i}");
            sfxObject.transform.SetParent(transform);
            AudioSource source = sfxObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.volume = sfxVolume;
            sfxPool.Enqueue(source); // Ǯ�� �߰�
        }
    }

    // ���� ���� ����
    private void ApplyVolumeSettings()
    {
        bgmSource.volume = bgmVolume;
        foreach (var source in sfxPool)
            source.volume = sfxVolume;
    }

    // BGM ���
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
            Debug.LogWarning($"BGM '{name}'��(��) ã�� �� �����ϴ�.");
        }
    }

    // BGM ����
    public void StopBGM() => bgmSource.Stop();

    // SFX ��� (Ǯ�� ���)
    public void PlaySFX(string name)
    {
        if (!sfxDictionary.TryGetValue(name, out var clip))
        {
            Debug.LogWarning($"SFX '{name}'��(��) ã�� �� �����ϴ�.");
            return;
        }

        AudioSource source = GetAvailableAudioSource();
        source.clip = clip;
        source.Play();
        StartCoroutine(ReturnToPoolAfterPlayback(source, clip.length));
    }

    // ��� ������ AudioSource �������� (ť ���)
    private AudioSource GetAvailableAudioSource()
    {
        AudioSource source = sfxPool.Dequeue(); // ť���� ����
        sfxPool.Enqueue(source);                // ���� ���� �ٽ� ť�� �߰�
        return source;
    }

    // ��� �Ϸ� �� AudioSource �ʱ�ȭ
    private System.Collections.IEnumerator ReturnToPoolAfterPlayback(AudioSource source, float duration)
    {
        yield return new WaitForSeconds(duration);
        source.clip = null; // �޸� ����ȭ�� ���� Ŭ�� ����
    }

    // BGM ���� ����
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        bgmSource.volume = bgmVolume;
    }

    // SFX ���� ����
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        foreach (var source in sfxPool)
            source.volume = sfxVolume;
    }

    // ��� ����� ���Ұ� ��ȯ
    public void MuteAll(bool mute)
    {
        bgmSource.mute = mute;
        foreach (var source in sfxPool)
            source.mute = mute;
    }

    // BGM ���Ұ� ���
    public void ToggleMuteBGM() => bgmSource.mute = !bgmSource.mute;

    // SFX ���Ұ� ���
    public void ToggleMuteSFX()
    {
        foreach (var source in sfxPool)
            source.mute = !source.mute;
    }
}
