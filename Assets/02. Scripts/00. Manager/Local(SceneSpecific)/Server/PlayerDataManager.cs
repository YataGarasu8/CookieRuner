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
    public static PlayerDataManager Instance; // �̱��� �ν��Ͻ�

    private const string SaveDataKey = "PlayerData"; // Ŭ���� ���� Ű
    private const string LeaderboardId = "Ranking"; // �������� ID
    public LeaderboardScoresPage leaderboard;

    // �÷��̾� ������ (ScriptableObject)
    public PlayerDataSO playerDataSO;

    void Awake()
    {
        // �̱��� ������ ����Ͽ� �ν��Ͻ��� ����
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
        // Unity Gaming Services �ʱ�ȭ
        await UnityServices.InitializeAsync();
        Debug.Log("Unity ���� �ʱ�ȭ �Ϸ�.");

        // �α��� Ȯ�� ��, �α��εǾ� ���� �ʴٸ� �͸� �α��� �õ�
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("�͸� �α��� ����. �÷��̾� ID: " + AuthenticationService.Instance.PlayerId);
            }
            catch (Exception ex)
            {
                Debug.LogError("�α��� ����: " + ex.Message);
                return;
            }
        }

        // Ŭ���忡�� �÷��̾� �����͸� �ҷ���
        bool loaded = await LoadPlayerDataAsync();
        if (!loaded)
        {
            // �����Ͱ� ������ �⺻�� ����
            SetDefaultPlayerData();
            Debug.Log("�⺻ �÷��̾� ������ ���� �Ϸ�.");
        }

        // �������忡�� ���� 10���� �÷��̾� �����͸� ������
        await GetTopPlayersAsync();

        
    }

    // Ŭ���忡�� �÷��̾� �����͸� �ҷ����� �Լ�
    public async Task<bool> LoadPlayerDataAsync()
    {
        try
        {
#pragma warning disable CS0618
            var loadedData = await CloudSaveService.Instance.Data.LoadAllAsync();
#pragma warning restore CS0618

            // ����� �����Ͱ� ������ false ��ȯ
            if (!loadedData.ContainsKey(SaveDataKey))
            {
                Debug.Log("����� �÷��̾� �����Ͱ� ����.");
                return false;
            }

            string jsonData = loadedData[SaveDataKey].ToString().Trim();

            // JSON ������ ����
            if (!jsonData.StartsWith("{") && !jsonData.StartsWith("["))
            {
                Debug.LogError("JSON ������ ������ �߸���. ��: " + jsonData);
                return false;
            }

            try
            {
                // JSON �����͸� PlayerDataPlain���� ��ȯ
                PlayerDataPlain tempData = JsonConvert.DeserializeObject<PlayerDataPlain>(jsonData);

                // ������ ���� �� ����
                if (!IsValidPlayerData(tempData))
                {
                    Debug.LogWarning("�ҷ��� �����Ͱ� ��ȿ���� ����. �⺻������ �ʱ�ȭ.");
                    SetDefaultPlayerData();
                    return false;
                }

                // ScriptableObject�� �� ����
                playerDataSO.playerName = tempData.playerName;
                playerDataSO.highScore = tempData.highScore;
                playerDataSO.gold = tempData.gold;

                Debug.Log($"�÷��̾� ������ �ҷ����� �Ϸ�: �̸� {playerDataSO.playerName}, ���� {playerDataSO.highScore}, ��� {playerDataSO.gold}");
            }
            catch (Exception innerEx)
            {
                Debug.LogError("JSON ������ȭ ����: " + innerEx.Message);
                SetDefaultPlayerData();
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError("Ŭ���忡�� ������ �ҷ����� ����: " + ex.Message);
            return false;
        }
    }

    // Ŭ���忡 �÷��̾� �����͸� �����ϴ� �Լ�
    public async Task SavePlayerDataAsync()
    {
        try
        {
            // PlayerDataSO �� PlayerDataPlain ��ȯ �� ����
            PlayerDataPlain tempData = new PlayerDataPlain
            {
                playerName = playerDataSO.playerName,
                highScore = playerDataSO.highScore,
                gold = playerDataSO.gold
            };

            if (!IsValidPlayerData(tempData))
            {
                Debug.LogError("��ȿ���� ���� ������. ������ ��ҵ�.");
                return;
            }

            // JSON ��ȯ �� ����
            string jsonData = JsonConvert.SerializeObject(tempData);
            var dataDict = new Dictionary<string, object> { { SaveDataKey, jsonData } };
#pragma warning disable CS0618
            await CloudSaveService.Instance.Data.ForceSaveAsync(dataDict);
#pragma warning restore CS0618
            Debug.Log("�÷��̾� �����Ͱ� Ŭ���忡 �����.");

            // �������� ���� ������Ʈ
            await UpdatePlayerScoreAsync(tempData.highScore);
        }
        catch (Exception ex)
        {
            Debug.LogError("�÷��̾� ������ ���� ����: " + ex.Message);
        }
    }


    // �������忡�� ���� 10���� �÷��̾� ������ �������� �Լ�
    public async Task GetTopPlayersAsync()
    {
        try
        {
            if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
            {
                Debug.LogError("Unity Services�� �ʱ�ȭ���� �ʾҽ��ϴ�.");
                return;
            }

            var scoresPage = await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId, new GetScoresOptions { Limit = 10 });

            if (scoresPage == null || scoresPage.Results == null || scoresPage.Results.Count == 0)
            {
                Debug.LogWarning("�������忡 ����� �����Ͱ� �����ϴ�.");
                return;
            }

            Debug.Log("���� 10�� ��ŷ �ҷ����� ����.");

            leaderboard = scoresPage;

            foreach (var score in scoresPage.Results)
            {
                Debug.Log($"����: {score.Rank}, �÷��̾� ID: {score.PlayerId}, ����: {score.Score}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("�������� ������ �ҷ����� ����: " + ex.Message);
        }
    }


    // ���� �÷��̾��� ������ �������忡 ������Ʈ�ϴ� �Լ�
    public async Task UpdatePlayerScoreAsync(int newScore)
    {
        try
        {
            int currentScore = 0; // �⺻�� 0�� ����

            try
            {
                // ���� ������ �������� �õ� (�������忡 �����Ͱ� ���� ��� ���� �߻� ����)
                var options = new GetPlayerScoreOptions();
                var playerScoreResponse = await LeaderboardsService.Instance.GetPlayerScoreAsync(LeaderboardId, options);
                currentScore = (int)playerScoreResponse.Score; // ���� ���� ��������
            }
            catch (Exception)
            {
                // �������忡�� ������ ã�� �� ���� ��� (ó�� ����ϴ� �÷��̾�)
                Debug.LogWarning("�������忡 ���� ������ ���� ���� ����մϴ�.");
            }

            // ���� �������� ���� ��쿡�� ������Ʈ (Ȥ�� ù ��° ���)
            if (newScore > currentScore)
            {
                await LeaderboardsService.Instance.AddPlayerScoreAsync(LeaderboardId, newScore);
                Debug.Log($"�������� ���� ������Ʈ ����: {newScore}");
            }
            else
            {
                Debug.Log($"�������� ������Ʈ ���ʿ�: ���� ���� {currentScore} �� ���ο� ���� {newScore}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("���� ������Ʈ ����: " + ex.Message);
        }
    }


    // �÷��̾� ������ ���� �Լ�
    private bool IsValidPlayerData(PlayerDataPlain data)
    {
        if (data == null) return false;
        if (string.IsNullOrWhiteSpace(data.playerName)) return false;
        if (data.highScore < 0) return false;
        if (data.gold < 0) return false;
        return true;
    }

    // �⺻ �÷��̾� ������ ���� �Լ�
    private void SetDefaultPlayerData()
    {
        playerDataSO.playerName = "Guest";
        playerDataSO.highScore = 0;
        playerDataSO.gold = 0;
    }
}