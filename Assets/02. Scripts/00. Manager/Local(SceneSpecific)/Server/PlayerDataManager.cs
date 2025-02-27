using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using System;
using Newtonsoft.Json;
public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;
    private const string SaveDataKey = "PlayerData"; // 저장할 데이터의 키

    // Inspector에서 할당할 ScriptableObject 에셋 (PlayerDataSO 에셋)
    public PlayerDataSO playerDataSO;

    void Awake()
    {
        // 싱글턴 패턴: 단일 인스턴스 유지
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async void Start()
    {
        // 유니티 서비스 초기화
        await UnityServices.InitializeAsync();
        Debug.Log("유니티 서비스 초기화 완료.");

        // 익명 로그인 (로그인되어 있지 않다면)
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("로그인 성공, 플레이어 ID: " + AuthenticationService.Instance.PlayerId);
            }
            catch (Exception ex)
            {
                Debug.LogError("로그인 실패: " + ex.Message);
                return;
            }
        }

        // Cloud Save에서 플레이어 데이터 불러오기 (Newtonsoft.Json 사용)
        bool loaded = await LoadPlayerDataAsync();
        if (!loaded)
        {
            // 저장된 데이터가 없으면 기본값 할당
            SetDefaultPlayerData();
            Debug.Log("저장된 플레이어 데이터가 없습니다. 기본 데이터 생성됨.");
        }
        else
        {
            Debug.Log($"불러온 데이터 - 이름: {playerDataSO.playerName}, 점수: {playerDataSO.highScore}, 골드: {playerDataSO.gold}");
        }
    }

    public async Task<bool> LoadPlayerDataAsync()
    {
        try
        {
#pragma warning disable CS0618
            var loadedData = await CloudSaveService.Instance.Data.LoadAllAsync();
#pragma warning restore CS0618
            if (loadedData.ContainsKey(SaveDataKey))
            {
                string jsonData = JsonConvert.SerializeObject(loadedData[SaveDataKey]);
                jsonData = jsonData.Trim();

                // JSON 데이터 형식 검증
                if (!jsonData.StartsWith("{") && !jsonData.StartsWith("["))
                {
                    Debug.LogError("처리된 JSON 데이터 형식이 잘못되었습니다. 값: " + jsonData);
                    return false;
                }

                try
                {
                    // JSON 데이터를 PlayerDataSO로 역직렬화
                    PlayerDataSO tempData = JsonConvert.DeserializeObject<PlayerDataSO>(jsonData);

                    // 데이터 검증
                    if (!IsValidPlayerData(tempData))
                    {
                        Debug.LogWarning("불러온 데이터가 유효하지 않아 기본값으로 초기화됩니다.");
                        SetDefaultPlayerData();
                        return false;
                    }

                    // ScriptableObject에 값 적용
                    playerDataSO.playerName = tempData.playerName;
                    playerDataSO.highScore = tempData.highScore;
                    playerDataSO.gold = tempData.gold;

                    Debug.Log($"플레이어 데이터 불러오기 완료 - 이름: {playerDataSO.playerName}, 점수: {playerDataSO.highScore}, 골드: {playerDataSO.gold}");
                }
                catch (Exception innerEx)
                {
                    Debug.LogError("JSON 역직렬화 오류: " + innerEx.Message);
                    SetDefaultPlayerData();
                    return false;
                }

                return true;
            }
            else
            {
                Debug.Log("Cloud Save에 저장된 플레이어 데이터가 존재하지 않습니다.");
                return false;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Newtonsoft.Json을 사용한 플레이어 데이터 불러오기 오류: " + ex.Message);
            return false;
        }
    }

    // 플레이어 데이터를 저장하는 함수 (ScriptableObject를 JSON으로 직렬화하여 저장)
    public async Task SavePlayerDataAsync()
    {
        try
        {
            // 저장 전에 데이터 검증
            if (!IsValidPlayerData(playerDataSO))
            {
                Debug.LogError("저장하려는 데이터가 유효하지 않습니다. 저장이 취소됩니다.");
                return;
            }

            // PlayerDataSO 객체를 JSON으로 변환
            string jsonData = JsonConvert.SerializeObject(playerDataSO);

            var dataToSave = new Dictionary<string, object>
            {
                { SaveDataKey, jsonData }
            };

#pragma warning disable CS0618
            await CloudSaveService.Instance.Data.ForceSaveAsync(dataToSave);
#pragma warning restore CS0618
            Debug.Log("플레이어 데이터가 클라우드에 성공적으로 저장되었습니다.");
        }
        catch (Exception ex)
        {
            Debug.LogError("플레이어 데이터 저장 오류: " + ex.Message);
        }
    }

    // 데이터 검증 함수 (유효한 값인지 확인)
    private bool IsValidPlayerData(PlayerDataSO data)
    {
        if (data == null) return false;
        if (string.IsNullOrWhiteSpace(data.playerName)) return false; // 이름이 비어있으면 안됨
        if (data.highScore < 0) return false; // 점수는 0 이상이어야 함
        if (data.gold < 0) return false; // 골드는 0 이상이어야 함
        return true;
    }

    // 기본 플레이어 데이터 설정 (잘못된 데이터가 있을 경우 기본값 할당)
    private void SetDefaultPlayerData()
    {
        playerDataSO.playerName = "Guest";
        playerDataSO.highScore = 0;
        playerDataSO.gold = 0;
    }
}
