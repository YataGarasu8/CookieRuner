using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;

public class LeaderboardsManager : MonoBehaviour
{
    public static LeaderboardsManager Instance;
    // ��ú��忡�� ������ �������� ID�� �Է��ϼ���.
    [SerializeField] private string leaderboardId = "Ranking";

    void Awake()
    {
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

    // �÷��̾��� ���� ���� �Լ�
    public async Task SubmitScoreAsync(long score)
    {
        try
        {
            // SubmitScoreAsync �Լ��� �������� ID�� ������ �Է¹޽��ϴ�.
            var response = await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardId, score);
            Debug.Log("Score submitted successfully. " + response);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error submitting score: " + ex.Message);
        }
    }

    // �������� �����͸� �ҷ����� �Լ�
    public async Task GetLeaderboardAsync()
    {
        try
        {
            // GetScoresAsync �Լ��� ��ü ��ŷ �����͸� �ҷ��ɴϴ�.
            LeaderboardScoresPage response = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId);
            Debug.Log("Leaderboard retrieved successfully.");
            // ���信�� �� �÷��̾��� ���� �� ������ ����մϴ�.
            foreach (var entry in response.Results)
            {
                Debug.Log($"Rank: {entry.Rank}, PlayerId: {entry.PlayerId}, Score: {entry.Score}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error retrieving leaderboard: " + ex.Message);
        }
    }
}
