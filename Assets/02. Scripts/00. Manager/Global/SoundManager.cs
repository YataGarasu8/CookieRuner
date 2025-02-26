using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance     // �̱��� �ν��Ͻ�
    {
        get
        {
            if (instance == null)
            {
                instance = new SoundManager();
                instance.Init(); // �ν��Ͻ� ���� �� �ʱ�ȭ �Լ� ȣ��
            }
            return instance;
        }
    }

    [Header("BGM Settings")]
    public AudioSource bgmSource;                 // �������(BGM)�� ����ϴ� AudioSource
    [Range(0f, 1f)] public float bgmVolume = 0.5f; // BGM ���� (0 ~ 1 ���� ���� ����)

    [Header("SFX Settings")]
    public int sfxPoolSize = 10;                  // ���ÿ� ����� �� �ִ� �ִ� SFX ä�� ��
    [Range(0f, 1f)] public float sfxVolume = 0.7f; // ȿ����(SFX) ����
    public List<AudioClip> sfxClips;              // �ν����Ϳ��� �߰��� �� �ִ� SFX ����� Ŭ�� ���

    private Dictionary<string, AudioClip> sfxDictionary = new Dictionary<string, AudioClip>(); // SFX Ŭ���� �̸����� ã�� ���� ����
    private Queue<AudioSource> sfxPool = new Queue<AudioSource>(); // SFX ����� ���� AudioSource Ǯ

    [Header("BGM Clips")]
    public List<AudioClip> bgmClips;              // �ν����Ϳ��� �߰��� �� �ִ� BGM ����� Ŭ�� ���
    private Dictionary<string, AudioClip> bgmDictionary = new Dictionary<string, AudioClip>(); // BGM Ŭ���� �̸����� ã�� ���� ����

    // �ʱ�ȭ �Լ�: �ν��Ͻ� ���� �� �ʿ��� �ʱ� ���� ����
    private void Init()
    {
        // GameObject ���� �� ����
        var soundManagerObject = new GameObject("SoundManager");
        UnityEngine.Object.DontDestroyOnLoad(soundManagerObject); // �� ��ȯ �ÿ��� ����

        // BGM�� AudioSource �߰� �� �ʱ� ����
        bgmSource = soundManagerObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.volume = bgmVolume;

        // SFX Ǯ �ʱ�ȭ
        InitializeSFXPool();

        // ���� ���ҽ� �ε�
        LoadSounds();

        // ���� ���� ����
        ApplyVolumeSettings();
    }

    // ������ �������� ��� ����� ������ �ε��ϰ� BGM/SFX ������ ���
    public void LoadSounds(string folderPath = "Sound")
    {
        // Resources ���� ������ ��θ� ����
        string soundFolderPath = Path.Combine(Application.dataPath, "Resources", folderPath);

        // ������ �������� ������ ���� �α� ��� �� �Լ� ����
        if (!Directory.Exists(soundFolderPath))
        {
            Debug.LogError($"���� {soundFolderPath}�� �������� �ʽ��ϴ�.");
            return;
        }

        // �ش� ���� �� ��� ���� ������ ����� ���� �˻�
        foreach (var file in Directory.GetFiles(soundFolderPath, "*.*", SearchOption.AllDirectories))
        {
            // ���� Ȯ���ڸ� �ҹ��ڷ� ��ȯ�Ͽ� ����� ���� ���� Ȯ��
            string extension = Path.GetExtension(file).ToLower();

            // �����ϴ� ����� ���˸� ó��
            if (extension == ".mp3" || extension == ".wav" || extension == ".ogg")
            {
                // Resources.Load�� ���� ��� ��� ���� (Ȯ���� ���� �� ��� ������ ����)
                string relativePath = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(file)).Replace("\\", "/");

                // Resources.Load�� ���� AudioClip �ε�
                AudioClip clip = Resources.Load<AudioClip>(relativePath);

                if (clip != null)
                {
                    string clipName = clip.name; // ����� Ŭ�� �̸����� Ű ����

                    // ���� �̸��� "bgm" ���� �� BGM ������ �߰�, �׷��� ������ SFX ������ �߰�
                    if (clipName.ToLower().Contains("bgm"))
                    {
                        bgmDictionary[clipName] = clip;
                        Debug.Log($"BGM �ε�: {clipName}");
                    }
                    else
                    {
                        sfxDictionary[clipName] = clip;
                        Debug.Log($"SFX �ε�: {clipName}");
                    }
                }
                else
                {
                    Debug.LogWarning($"���� ������ �ε����� ���߽��ϴ�: {relativePath}");
                }
            }
        }
    }

    // SFX ����� ���� AudioSource ��ü���� Ǯ�� ������� �ʱ�ȭ
    private void InitializeSFXPool()
    {
        for (int i = 0; i < sfxPoolSize; i++)
        {
            // AudioSource�� ���� ���ο� GameObject ����
            GameObject sfxObject = new GameObject($"SFXSource_{i}");
            sfxObject.transform.SetParent(transform); // SoundManager ������Ʈ�� �ڽ����� �߰�

            // AudioSource ������Ʈ �߰� �� ����
            AudioSource source = sfxObject.AddComponent<AudioSource>();
            source.playOnAwake = false; // �ڵ� ��� ����
            source.volume = sfxVolume;  // �ʱ� ���� ����

            // AudioSource�� ť�� �߰� (Ǯ��)
            sfxPool.Enqueue(source);
        }
    }

    // �ʱ� ���� ������ ���� (BGM �� SFX)
    private void ApplyVolumeSettings()
    {
        bgmSource.volume = bgmVolume; // BGM ���� ����

        // ��� SFX AudioSource�� ���� ����
        foreach (var source in sfxPool)
            source.volume = sfxVolume;
    }

    // ������ �̸��� BGM�� ���
    public void PlayBGM(string name, bool loop = true)
    {
        // BGM �������� �ش� �̸��� Ŭ�� �˻�
        if (bgmDictionary.TryGetValue(name, out var clip))
        {
            bgmSource.clip = clip;   // BGM �ҽ��� Ŭ�� �Ҵ�
            bgmSource.loop = loop;   // �ݺ� ��� ���� ����
            bgmSource.Play();        // BGM ���
        }
        else
        {
            Debug.LogWarning($"BGM '{name}'��(��) ã�� �� �����ϴ�."); // Ŭ�� �̹߰� �� ��� ���
        }
    }

    // ���� ��� ���� BGM�� ����
    public void StopBGM() => bgmSource.Stop();

    // ������ �̸��� SFX�� ��� (Ǯ�� ���)
    public void PlaySFX(string name)
    {
        // SFX �������� �ش� �̸��� Ŭ�� �˻�
        if (!sfxDictionary.TryGetValue(name, out var clip))
        {
            Debug.LogWarning($"SFX '{name}'��(��) ã�� �� �����ϴ�."); // Ŭ�� �̹߰� �� ��� ���
            return;
        }

        // ��� ������ AudioSource ��������
        AudioSource source = GetAvailableAudioSource();
        source.clip = clip; // ����� Ŭ�� �Ҵ�
        source.Play();      // ȿ���� ���

        // ��� �Ϸ� �� AudioSource �ʱ�ȭ�� ���� �ڷ�ƾ ����
        StartCoroutine(ReturnToPoolAfterPlayback(source, clip.length));
    }

    // ��� ������ AudioSource�� ť���� ��������
    private AudioSource GetAvailableAudioSource()
    {
        AudioSource source = sfxPool.Dequeue(); // ť���� AudioSource ������
        sfxPool.Enqueue(source);                // ������ ���� �ٽ� ť�� �߰�
        return source;
    }

    // SFX ��� �� ���� �ð��� ������ AudioSource �ʱ�ȭ
    private System.Collections.IEnumerator ReturnToPoolAfterPlayback(AudioSource source, float duration)
    {
        yield return new WaitForSeconds(duration); // Ŭ�� ���̸�ŭ ���
        source.clip = null;                        // �޸� ����ȭ�� ���� Ŭ�� ����
    }

    // BGM ���� ����
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume); // ���� ���� 0 ~ 1 ���̷� ����
        bgmSource.volume = bgmVolume;      // BGM �ҽ��� ����
    }

    // SFX ���� ����
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume); // ���� ���� 0 ~ 1 ���̷� ����
        foreach (var source in sfxPool)
            source.volume = sfxVolume;     // ��� SFX AudioSource�� ����
    }

    // ��� ����� ���Ұ� �Ǵ� ����
    public void MuteAll(bool mute)
    {
        bgmSource.mute = mute; // BGM ���Ұ� ����
        foreach (var source in sfxPool)
            source.mute = mute; // ��� SFX ���Ұ� ����
    }

    // BGM ���Ұ� ���
    public void ToggleMuteBGM() => bgmSource.mute = !bgmSource.mute;

    // SFX ���Ұ� ���
    public void ToggleMuteSFX()
    {
        foreach (var source in sfxPool)
            source.mute = !source.mute; // �� SFX AudioSource�� ���Ұ� ���� ���
    }
}
