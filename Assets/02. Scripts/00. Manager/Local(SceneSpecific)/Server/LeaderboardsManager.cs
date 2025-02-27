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
    // 대시보드에서 생성한 리더보드 ID를 입력하세요.
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

    // 플레이어의 점수 제출 함수
    public async Task SubmitScoreAsync(long score)
    {
        try
        {
            // SubmitScoreAsync 함수는 리더보드 ID와 점수를 입력받습니다.
            var response = await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardId, score);
            Debug.Log("Score submitted successfully. " + response);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error submitting score: " + ex.Message);
        }
    }

    // 리더보드 데이터를 불러오는 함수
    public async Task GetLeaderboardAsync()
    {
        try
        {
            // GetScoresAsync 함수로 전체 랭킹 데이터를 불러옵니다.
            LeaderboardScoresPage response = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId);
            Debug.Log("Leaderboard retrieved successfully.");
            // 응답에서 각 플레이어의 점수 및 순위를 출력합니다.
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
