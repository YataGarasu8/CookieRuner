using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using TMPro;
using UnityEngine;



public class GameManager
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

    //게임오버 변수
    public bool IsGameOver { get; set; }

    public string charSelect = CharacterSelect.Default.ToString();

    // 초기화 함수: 인스턴스 생성 시 필요한 초기 설정 수행
    private void Init()
    {

    }
    public int Money { get; set; } //플레이어가 보유한 골드의 총량
    public int getMoney;//
    int startMoney;
    public string PlayerName {  get; set; }

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
