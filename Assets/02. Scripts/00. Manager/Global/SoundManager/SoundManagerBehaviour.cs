using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
