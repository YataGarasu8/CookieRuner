using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using UnityEngine.Networking;


// MonoBehaviour�� ������� ������, ���������� SoundManagerBehaviour�� ���� Unity ���(transform, StartCoroutine ��)�� ����� �� ����
// BGM �� SFX ���, ���� ����, ���� �ε� �� Ǯ�� ����� ����
public class SoundManager
{
    // �̱��� �ν��Ͻ��� �����ϴ� ���� ����
    private static SoundManager instance;

    // �ܺο��� ���� ������ �̱��� �ν��Ͻ� ������Ƽ
    public static SoundManager Instance
    {
        get
        {
            // �ν��Ͻ��� �������� ������ ���� �� �ʱ�ȭ
            if (instance == null)
            {
                instance = new SoundManager();
                instance.Init(); // �ʱ�ȭ �Լ� ȣ��
            }
            return instance; // �ν��Ͻ� ��ȯ
        }
    }

    // BGM �� SFX ���� ���� ����
    public AudioSource bgmSource;                 // BGM ����� AudioSource
    public float bgmVolume = 0.5f;                // BGM ���� (0 ~ 1)
    public int sfxPoolSize = 10;                  // ���ÿ� ��� ������ SFX ä�� ��
    public float sfxVolume = 0.7f;                // SFX ���� (0 ~ 1)

    // ���� ������ �����ϴ� �÷���
    private Dictionary<string, AudioClip> sfxDictionary = new Dictionary<string, AudioClip>(); // SFX �̸�-Ŭ�� ����
    private Dictionary<string, AudioClip> bgmDictionary = new Dictionary<string, AudioClip>(); // BGM �̸�-Ŭ�� ����

    // SFX ����� ���� AudioSource Ǯ
    private Queue<AudioSource> sfxPool = new Queue<AudioSource>();

    // �ʱ�ȭ �Լ�: �ν��Ͻ� ���� �� �� ���� ȣ��
    private void Init()
    {
        // GameObject ���� �� SoundManagerBehaviour �߰� (�ڷ�ƾ�� transform ����� ����)
        var soundManagerObject = new GameObject("SoundManager");
        UnityEngine.Object.DontDestroyOnLoad(soundManagerObject); // �� ��ȯ �� ����
        soundManagerObject.AddComponent<SoundManagerBehaviour>(); // MonoBehaviour ��� ����

        // BGM�� AudioSource �ʱ�ȭ �� ����
        bgmSource = soundManagerObject.AddComponent<AudioSource>();
        bgmSource.loop = true; // �⺻������ �ݺ� ��� ����
        bgmSource.volume = bgmVolume; // �ʱ� BGM ���� ����

        // SFX ����� ���� AudioSource Ǯ �ʱ�ȭ
        InitializeSFXPool();

        // ���� ���� �ε�
        LoadSounds();

        // �ʱ� ���� ����
        ApplyVolumeSettings();
    }



    // ������ �������� ����� ������ �ε��Ͽ� ������ ���
    public void LoadSounds(string folderPath = @"00. Art/11. Sound")
    {
        // ������Ʈ ��Ʈ���� ���� ��� ���� (Application.dataPath�� 'Assets' ��� ��ȯ)
        string soundFolderPath = Path.Combine(Application.dataPath, folderPath);

        // ���� ���� ���� Ȯ��
        if (!Directory.Exists(soundFolderPath))
        {
            Debug.LogError($"���� {soundFolderPath}�� �������� �ʽ��ϴ�.");
            return;
        }

        // �����Ǵ� ����� ���� Ȯ���� ���
        string[] supportedExtensions = { ".mp3", ".wav", ".ogg" };

        // ��� ����� ���� �˻�
        foreach (var file in Directory.GetFiles(soundFolderPath, "*.*", SearchOption.AllDirectories))
        {
            string extension = Path.GetExtension(file).ToLower();

            // �������� �ʴ� ������ �ǳʶ�
            if (!supportedExtensions.Contains(extension))
                continue;

            string fileName = Path.GetFileNameWithoutExtension(file); // ���ϸ�(Ȯ���� ����)
            string fileUri = $"file://{file.Replace("\\", "/")}";     // ��� ������ ��ȯ �� URI ����

            // �ڷ�ƾ���� �񵿱� ����� �ε�
            SoundManagerBehaviour.Instance.RunCoroutine(LoadAudioClip(fileUri, fileName));
        }
    }

    // ����� ������ �񵿱�� �ε��ϴ� �ڷ�ƾ
    private System.Collections.IEnumerator LoadAudioClip(string fileUri, string clipName)
    {
        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(fileUri, AudioType.UNKNOWN))
        {
            yield return request.SendWebRequest(); // ��û ���

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"����� �ε� ����: {clipName} - {request.error}");
                yield break;
            }

            // AudioClip ���� �� ���
            AudioClip clip = DownloadHandlerAudioClip.GetContent(request);

            // ��� ��� ��� (bgm ���� ���� �� BGM ���)
            if (fileUri.ToLower().Contains("/bgm/") || clipName.ToLower().Contains("bgm"))
            {
                bgmDictionary[clipName] = clip;
                Debug.Log($"BGM �ε� �Ϸ�: {clipName}");
            }
            else
            {
                sfxDictionary[clipName] = clip;
                Debug.Log($"SFX �ε� �Ϸ�: {clipName}");
            }
        }
    }



    // SFX ����� ���� AudioSource Ǯ �ʱ�ȭ
    private void InitializeSFXPool()
    {
        for (int i = 0; i < sfxPoolSize; i++)
        {
            // ���ο� GameObject ���� �� AudioSource �߰�
            GameObject sfxObject = new GameObject($"SFXSource_{i}");
            sfxObject.transform.SetParent(SoundManagerBehaviour.Instance.transform); // Helper ������Ʈ�� ����

            // AudioSource ����
            AudioSource source = sfxObject.AddComponent<AudioSource>();
            source.playOnAwake = false; // �ڵ� ��� ��Ȱ��ȭ
            source.volume = sfxVolume;  // �ʱ� SFX ���� ����

            // Ǯ�� AudioSource �߰�
            sfxPool.Enqueue(source);
        }
    }

    // ��� AudioSource�� ���� ����
    private void ApplyVolumeSettings()
    {
        bgmSource.volume = bgmVolume; // BGM ���� ����

        // ��� SFX AudioSource�� ���� ����
        foreach (var source in sfxPool)
        {
            source.volume = sfxVolume;
        }
    }

    // ������ �̸��� BGM ���
    public void PlayBGM(string name, bool loop = true)
    {
        // BGM Ŭ�� �˻�
        if (bgmDictionary.TryGetValue(name, out var clip))
        {
            bgmSource.clip = clip;   // AudioSource�� Ŭ�� �Ҵ�
            bgmSource.loop = loop;   // �ݺ� ��� ���� ����
            bgmSource.Play();        // BGM ��� ����
        }
        else
        {
            Debug.LogWarning($"BGM '{name}'��(��) ã�� �� �����ϴ�."); // �ش� �̸��� Ŭ���� ���� �� ��� ���
        }
    }

    // ���� ��� ���� BGM ����
    public void StopBGM()
    {
        bgmSource.Stop(); // BGM ��� ����
    }

    // ������ �̸��� SFX ���
    public void PlaySFX(string name)
    {
        // SFX Ŭ�� �˻�
        if (!sfxDictionary.TryGetValue(name, out var clip))
        {
            Debug.LogWarning($"SFX '{name}'��(��) ã�� �� �����ϴ�."); // Ŭ�� �̹߰� �� ��� ���
            return;
        }

        // ��� ������ AudioSource ��������
        AudioSource source = GetAvailableAudioSource();
        source.clip = clip; // Ŭ�� �Ҵ�
        source.Play();      // ȿ���� ���

        // �ڷ�ƾ ������ ���� Helper Ŭ������ ���
        SoundManagerBehaviour.Instance.RunCoroutine(ReturnToPoolAfterPlayback(source, clip.length));
    }

    // ��� ������ AudioSource �������� (��� ��� ���� ��� ť�� ù ��° ���)
    private AudioSource GetAvailableAudioSource()
    {
        AudioSource source = sfxPool.Dequeue(); // ť���� AudioSource ��������
        sfxPool.Enqueue(source);                // ������ ���� �ٽ� ť�� �߰�
        return source;                          // ������ AudioSource ��ȯ
    }

    // ȿ���� ��� �� ���� �ð� ��� �� Ŭ�� �ʱ�ȭ (�޸� ����ȭ)
    private System.Collections.IEnumerator ReturnToPoolAfterPlayback(AudioSource source, float duration)
    {
        yield return new WaitForSeconds(duration); // ������ �ð� ���� ���
        source.clip = null;                        // Ŭ�� ����
    }

    // BGM ���� ����
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume); // 0 ~ 1 ���� ����
        bgmSource.volume = bgmVolume;      // ���� ����
    }

    // SFX ���� ����
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume); // 0 ~ 1 ���� ����
        foreach (var source in sfxPool)
        {
            source.volume = sfxVolume; // ��� SFX AudioSource�� ����
        }
    }

    // ��ü ����� ���Ұ� �� ����
    public void MuteAll(bool mute)
    {
        bgmSource.mute = mute; // BGM ���Ұ� ����
        foreach (var source in sfxPool)
        {
            source.mute = mute; // ��� SFX ���Ұ� ����
        }
    }

    // BGM ���Ұ� ���
    public void ToggleMuteBGM()
    {
        bgmSource.mute = !bgmSource.mute; // ���� ���� ����
    }

    // SFX ���Ұ� ���
    public void ToggleMuteSFX()
    {
        foreach (var source in sfxPool)
        {
            source.mute = !source.mute; // ���� ���� ����
        }
    }
}