using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using System;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using Newtonsoft.Json;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance; // 싱글턴 인스턴스

    private const string SaveDataKey = "PlayerData"; // 클라우드 저장 키
    private const string LeaderboardId = "Ranking"; // 리더보드 ID
    public LeaderboardScoresPage leaderboard;

    // 플레이어 데이터 (ScriptableObject)
    public PlayerDataSO playerDataSO;

    void Awake()
    {
        // 싱글턴 패턴을 사용하여 인스턴스를 유지
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
        // Unity Gaming Services 초기화
        await UnityServices.InitializeAsync();
        Debug.Log("Unity 서비스 초기화 완료.");

        // 로그인 확인 후, 로그인되어 있지 않다면 익명 로그인 시도
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("익명 로그인 성공. 플레이어 ID: " + AuthenticationService.Instance.PlayerId);
            }
            catch (Exception ex)
            {
                Debug.LogError("로그인 실패: " + ex.Message);
                return;
            }
        }

        // 클라우드에서 플레이어 데이터를 불러옴
        bool loaded = await LoadPlayerDataAsync();
        if (!loaded)
        {
            // 데이터가 없으면 기본값 설정
            SetDefaultPlayerData();
            Debug.Log("기본 플레이어 데이터 설정 완료.");
        }

        // 리더보드에서 상위 10명의 플레이어 데이터를 가져옴
        await GetTopPlayersAsync();

        
    }

    // 클라우드에서 플레이어 데이터를 불러오는 함수
    public async Task<bool> LoadPlayerDataAsync()
    {
        try
        {
#pragma warning disable CS0618
            var loadedData = await CloudSaveService.Instance.Data.LoadAllAsync();
#pragma warning restore CS0618

            // 저장된 데이터가 없으면 false 반환
            if (!loadedData.ContainsKey(SaveDataKey))
            {
                Debug.Log("저장된 플레이어 데이터가 없음.");
                return false;
            }

            string jsonData = loadedData[SaveDataKey].ToString().Trim();

            // JSON 데이터 검증
            if (!jsonData.StartsWith("{") && !jsonData.StartsWith("["))
            {
                Debug.LogError("JSON 데이터 형식이 잘못됨. 값: " + jsonData);
                return false;
            }

            try
            {
                // JSON 데이터를 PlayerDataPlain으로 변환
                PlayerDataPlain tempData = JsonConvert.DeserializeObject<PlayerDataPlain>(jsonData);

                // 데이터 검증 후 적용
                if (!IsValidPlayerData(tempData))
                {
                    Debug.LogWarning("불러온 데이터가 유효하지 않음. 기본값으로 초기화.");
                    SetDefaultPlayerData();
                    return false;
                }

                // ScriptableObject에 값 적용
                playerDataSO.playerName = tempData.playerName;
                playerDataSO.highScore = tempData.highScore;
                playerDataSO.gold = tempData.gold;

                Debug.Log($"플레이어 데이터 불러오기 완료: 이름 {playerDataSO.playerName}, 점수 {playerDataSO.highScore}, 골드 {playerDataSO.gold}");
            }
            catch (Exception innerEx)
            {
                Debug.LogError("JSON 역직렬화 오류: " + innerEx.Message);
                SetDefaultPlayerData();
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError("클라우드에서 데이터 불러오기 실패: " + ex.Message);
            return false;
        }
    }

    // 클라우드에 플레이어 데이터를 저장하는 함수
    public async Task SavePlayerDataAsync()
    {
        try
        {
            // PlayerDataSO → PlayerDataPlain 변환 후 검증
            PlayerDataPlain tempData = new PlayerDataPlain
            {
                playerName = playerDataSO.playerName,
                highScore = playerDataSO.highScore,
                gold = playerDataSO.gold
            };

            if (!IsValidPlayerData(tempData))
            {
                Debug.LogError("유효하지 않은 데이터. 저장이 취소됨.");
                return;
            }

            // JSON 변환 후 저장
            string jsonData = JsonConvert.SerializeObject(tempData);
            var dataDict = new Dictionary<string, object> { { SaveDataKey, jsonData } };
#pragma warning disable CS0618
            await CloudSaveService.Instance.Data.ForceSaveAsync(dataDict);
#pragma warning restore CS0618
            Debug.Log("플레이어 데이터가 클라우드에 저장됨.");

            // 리더보드 점수 업데이트
            await UpdatePlayerScoreAsync(tempData.highScore);
        }
        catch (Exception ex)
        {
            Debug.LogError("플레이어 데이터 저장 실패: " + ex.Message);
        }
    }


    // 리더보드에서 상위 10명의 플레이어 정보를 가져오는 함수
    public async Task GetTopPlayersAsync()
    {
        try
        {
            if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
            {
                Debug.LogError("Unity Services가 초기화되지 않았습니다.");
                return;
            }

            var scoresPage = await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId, new GetScoresOptions { Limit = 10 });

            if (scoresPage == null || scoresPage.Results == null || scoresPage.Results.Count == 0)
            {
                Debug.LogWarning("리더보드에 저장된 데이터가 없습니다.");
                return;
            }

            Debug.Log("상위 10명 랭킹 불러오기 성공.");

            leaderboard = scoresPage;

            foreach (var score in scoresPage.Results)
            {
                Debug.Log($"순위: {score.Rank}, 플레이어 ID: {score.PlayerId}, 점수: {score.Score}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("리더보드 데이터 불러오기 실패: " + ex.Message);
        }
    }


    // 현재 플레이어의 점수를 리더보드에 업데이트하는 함수
    public async Task UpdatePlayerScoreAsync(int newScore)
    {
        try
        {
            int currentScore = 0; // 기본값 0점 설정

            try
            {
                // 기존 점수를 가져오기 시도 (리더보드에 데이터가 없을 경우 예외 발생 가능)
                var options = new GetPlayerScoreOptions();
                var playerScoreResponse = await LeaderboardsService.Instance.GetPlayerScoreAsync(LeaderboardId, options);
                currentScore = (int)playerScoreResponse.Score; // 기존 점수 가져오기
            }
            catch (Exception)
            {
                // 리더보드에서 점수를 찾을 수 없는 경우 (처음 등록하는 플레이어)
                Debug.LogWarning("리더보드에 기존 점수가 없어 새로 등록합니다.");
            }

            // 기존 점수보다 높을 경우에만 업데이트 (혹은 첫 번째 등록)
            if (newScore > currentScore)
            {
                await LeaderboardsService.Instance.AddPlayerScoreAsync(LeaderboardId, newScore);
                Debug.Log($"리더보드 점수 업데이트 성공: {newScore}");
            }
            else
            {
                Debug.Log($"리더보드 업데이트 불필요: 기존 점수 {currentScore} ≥ 새로운 점수 {newScore}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("점수 업데이트 실패: " + ex.Message);
        }
    }


    // 플레이어 데이터 검증 함수
    private bool IsValidPlayerData(PlayerDataPlain data)
    {
        if (data == null) return false;
        if (string.IsNullOrWhiteSpace(data.playerName)) return false;
        if (data.highScore < 0) return false;
        if (data.gold < 0) return false;
        return true;
    }

    // 기본 플레이어 데이터 설정 함수
    private void SetDefaultPlayerData()
    {
        playerDataSO.playerName = "Guest";
        playerDataSO.highScore = 0;
        playerDataSO.gold = 0;
    }
}