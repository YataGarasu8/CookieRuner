using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance     // 싱글톤 인스턴스
    {
        get
        {
            if (instance == null)
            {
                instance = new GameManager();
                instance.Init(); // 인스턴스 생성 시 초기화 함수 호출
            }
            return instance;
        }
    }

    //보너스아이템 획득여부
    private bool isGetLJH = false;
    private bool isGetLKW = false;
    private bool isGetJSW = false;
    private bool isGetLYJ = false;
    private bool isGetKYJ = false;

    // 초기화 함수: 인스턴스 생성 시 필요한 초기 설정 수행
    private void Init()
    {

    }
    public int Money { get; set; } // 유저 골드재화

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 중복 방지
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환 시 유지
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
                Debug.Log("보너스아이템디폴트");
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
