using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //���ʽ������� ȹ�濩��
    private bool isGetLJH = false;
    private bool isGetLKW = false;
    private bool isGetJSW = false;
    private bool isGetLYJ = false;
    private bool isGetKYJ = false;

    public int Money { get; set; } // ���� �����ȭ

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // �ߺ� ����
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // �� ��ȯ �� ����
    }

    public bool IsGetAllBonusItem()
    {
        if (isGetJSW && isGetKYJ && isGetLJH && isGetLKW && isGetLYJ)
        {
            return true;
        }
        else
            return false;
    }

    public bool GetBonusItem(MoluName name)
    {
        switch (name)
        {
            case MoluName.LJH:
                if (!isGetLJH)
                {
                    isGetLJH = true;
                    return false;
                }
                else { return true; }
            case MoluName.LKW:
                if (!isGetLKW)
                {
                    isGetLKW = true;
                    return false;
                }
                else { return true; }
            case MoluName.LYJ:
                if (!isGetLYJ)
                {
                    isGetLYJ = true;
                    return false;
                }
                else { return true; }
            case MoluName.KYJ:
                if (!isGetKYJ)
                {
                    isGetKYJ = true;
                    return false;
                }
                else { return true; }
            case MoluName.JSW:
                if (!isGetJSW)
                {
                    isGetJSW = true;
                    return false;
                }
                else { return true; }

            default:
                Debug.Log("���ʽ������۵���Ʈ");
                return false;
        }
    }

    public void ResetBonusItem()
    {
        isGetJSW = false;
        isGetKYJ = false;
        isGetLKW = false;
        isGetLYJ = false;
        isGetLJH = false;
    }
}
