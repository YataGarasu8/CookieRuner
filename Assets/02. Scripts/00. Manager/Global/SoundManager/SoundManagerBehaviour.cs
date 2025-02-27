using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// ����: MonoBehaviour�� ����ϴ� ���� Ŭ����
// SoundManager���� ����� �� ���� �ڷ�ƾ �� transform ���� ����� �����ϱ� ���� ���
// �̱��� ������ �����Ͽ� �ߺ� ������ �����ϰ�, �� ��ȯ �ÿ��� ����

public class SoundManagerBehaviour : MonoBehaviour
{
    // �̱��� �ν��Ͻ��� �����ϴ� ���� ����
    private static SoundManagerBehaviour instance;

    // �ܺο��� ���� ������ �̱��� �ν��Ͻ� ������Ƽ
    public static SoundManagerBehaviour Instance
    {
        get
        {
            // �ν��Ͻ��� �������� ������ ����
            if (instance == null)
            {
                var obj = new GameObject("SoundManagerHelper"); // GameObject ����
                instance = obj.AddComponent<SoundManagerBehaviour>(); // ������Ʈ �߰�
                DontDestroyOnLoad(obj); // �� ��ȯ �ÿ��� �ı����� �ʵ��� ����
            }
            return instance; // �ν��Ͻ� ��ȯ
        }
    }

    // SoundManager���� �ڷ�ƾ ���� �� ȣ���ϴ� �Լ�
    public void RunCoroutine(System.Collections.IEnumerator coroutine)
    {
        StartCoroutine(coroutine); // ���޵� �ڷ�ƾ ����
    }

    private void Awake()
    {
        
    }

    private void Start()
    {
        SoundManager.Instance.LoadSounds();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            SoundManager.Instance.StopBGM();
            SoundManager.Instance.PlayBGM("GameSceneBGM01");
        }
        if(scene.name == "LobbyScene")
        {
            SoundManager.Instance.StopBGM();
            SoundManager.Instance.PlayBGM("LobbyBGM01");
        }
        if(scene.name == "StoreScene")
        {
            SoundManager.Instance.StopBGM();
            SoundManager.Instance.PlayBGM("StoreSceneBGM01");
        }
    }
}
