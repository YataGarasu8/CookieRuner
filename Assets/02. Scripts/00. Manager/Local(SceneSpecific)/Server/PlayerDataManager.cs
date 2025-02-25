using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        bool loaded = await LoadPlayerDataWithNewtonsoftAsync();
        if (!loaded)
        {
            // 저장된 데이터가 없으면 기본값 할당
            playerDataSO.highScore = 0;
            Debug.Log("저장된 플레이어 데이터가 없습니다. 기본 데이터 생성됨.");
        }
        else
        {
            Debug.Log($"불러온 데이터 - 최고 점수: {playerDataSO.highScore}");
        }
    }
    public async Task<bool> LoadPlayerDataWithNewtonsoftAsync()
    {
        try
        {
#pragma warning disable CS0618
            var loadedData = await CloudSaveService.Instance.Data.LoadAllAsync();
#pragma warning restore CS0618
            if (loadedData.ContainsKey(SaveDataKey))
            {
                // 불러온 데이터를 rawData 변수에 저장 (타입은 object)
                object rawData = loadedData[SaveDataKey];
                string jsonData = "";

                // rawData가 string이면 바로 사용합니다.
                if (rawData is string)
                {
                    jsonData = (string)rawData;
                    Debug.Log("불러온 데이터가 string 타입입니다: " + jsonData);
                }
                else
                {
                    // 그렇지 않다면, rawData의 ToString()이나 SerializeObject를 사용해 봅니다.
                    // (주의: ToString()은 객체의 타입 이름을 반환할 수 있으므로 SerializeObject를 사용하는 것이 좋습니다)
                    jsonData = JsonConvert.SerializeObject(rawData);
                    Debug.Log("SerializeObject를 사용하여 변환한 JSON 데이터: " + jsonData);
                }

                // 불필요한 공백 제거
                jsonData = jsonData.Trim();

                // 만약 jsonData가 이중 인코딩된 경우 (따옴표로 감싸져 있다면) 처리
                if (jsonData.StartsWith("\"") && jsonData.EndsWith("\""))
                {
                    Debug.Log("이중 인코딩 감지됨. 추가 따옴표 제거합니다.");
                    jsonData = jsonData.Substring(1, jsonData.Length - 2);
                    jsonData = jsonData.Replace("\\\"", "\"");
                }

                Debug.Log("최종 처리된 JSON 데이터: " + jsonData);
                Debug.Log("JSON 데이터 길이: " + jsonData.Length);

                // JSON 문자열이 올바른 형식인지 확인 ('{' 또는 '['로 시작하는지)
                if (!jsonData.StartsWith("{") && !jsonData.StartsWith("["))
                {
                    Debug.LogError("처리된 JSON 데이터가 '{' 또는 '['로 시작하지 않습니다. 값: " + jsonData);
                    return false;
                }

                try
                {
                    // 일반 데이터 클래스로 역직렬화 (예: PlayerDataPlain)
                    PlayerDataPlain tempData = JsonConvert.DeserializeObject<PlayerDataPlain>(jsonData);
                    // ScriptableObject에 값 복사
                    PlayerDataManager.Instance.playerDataSO.highScore = tempData.highScore;
                    Debug.Log("플레이어 데이터 역직렬화 및 ScriptableObject 업데이트 성공.");
                }
                catch (Exception innerEx)
                {
                    Debug.LogError("역직렬화 오류: " + innerEx.Message);
                    throw;
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
            // ScriptableObject를 JSON 문자열로 직렬화 (JsonUtility 사용)
            string jsonData = JsonUtility.ToJson(playerDataSO);
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
}