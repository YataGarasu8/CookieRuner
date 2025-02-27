using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.CloudSave;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;          // 현재 점수 표시
    public TextMeshProUGUI highScoresText;     // 역대 최고 점수 표시

    [Header("Settings")]
    public int maxHighScores = 10;  // 최대 순위 개수

    private int currentScore = 0;   // 현재 점수
    private List<int> highScores = new List<int>(); // 로컬 최고 점수 목록

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        LoadHighScores();
        //UpdateUI();
    }

    // 게임 도중 점수를 추가하는 함수
    public void AddScore(int amount)
    {
        currentScore += amount;
        //UpdateUI();
        Debug.Log($"현재 점수: {currentScore}");

        if (scoreText != null)
            scoreText.text = $"현재 점수: {currentScore}";
    }
    //void Update()
    //{
    //    // Q 키가 눌렸을 경우 SaveCurrentScore() 실행
    //    if (Input.GetKeyDown(KeyCode.Q))
    //    {
    //        Debug.Log("저장 시작");
    //        SaveCurrentScore();
    //    }
    //}

    // 게임 종료 시 호출되는 함수 (한 번만 클라우드에 저장)
    public void SaveCurrentScore()
    {
        // 로컬 최고 점수 목록 업데이트
        highScores.Add(currentScore);
        highScores.Sort((a, b) => b.CompareTo(a)); // 내림차순 정렬
        if (highScores.Count > maxHighScores)
            highScores.RemoveRange(maxHighScores, highScores.Count - maxHighScores);
        SaveHighScores();
        UpdateUI();

        // PlayerDataManager의 ScriptableObject와 비교하여 최고 점수 갱신
        if (PlayerDataManager.Instance != null && PlayerDataManager.Instance.playerDataSO != null)
        {
            PlayerDataManager.Instance.playerDataSO.highScore = Mathf.Max(currentScore, PlayerDataManager.Instance.playerDataSO.highScore);
            Debug.Log("클라우드에 저장할 최고 점수를 업데이트합니다: " + currentScore);
            // 게임 종료 시 한 번만 클라우드에 저장
#pragma warning disable CS4014
            PlayerDataManager.Instance.SavePlayerDataAsync();
#pragma warning restore CS4014
        }
        else
        {
            Debug.LogError("PlayerDataManager 또는 playerDataSO가 null입니다!");
        }
    }

    // UI 업데이트 함수
    private void UpdateUI()
    {
#pragma warning disable CS4014
        using var _ =
#pragma warning disable CS4014
        PlayerDataManager.Instance.GetTopPlayersAsync();
        PlayerDataManager.Instance.LoadPlayerDataAsync();
#pragma warning restore CS4014

        TextMeshProUGUI proUGUI = new TextMeshProUGUI();
        
        foreach (var score in PlayerDataManager.Instance.leaderboard.Results)
        {
            proUGUI.text += $"{score.Rank+1}등  점수 : {score.Score}\n";
            Debug.Log($"순위: {score.Rank}, 플레이어 ID: {score.PlayerId}, 점수: {score.Score}");
        }

        highScoresText.text = "";
        highScoresText.text += proUGUI.text;

        if (scoreText != null)
            scoreText.text = $"현재 점수: {currentScore}";

        //if (highScoresText != null)
        //{
        //    highScoresText.text = "역대 최고 점수:\n";
        //    for (int i = 0; i < highScores.Count; i++)
        //    {
        //        highScoresText.text += $"{i + 1}. {highScores[i]}점\n";
        //    }
        //}
    }

    // 로컬 최고 점수 로드 (PlayerPrefs 사용)
    private void LoadHighScores()
    {
        highScores.Clear();
        for (int i = 0; i < maxHighScores; i++)
        {
            int savedScore = PlayerPrefs.GetInt($"HighScore{i}", 0);
            if (savedScore > 0)
                highScores.Add(savedScore);
        }
        highScores.Sort((a, b) => b.CompareTo(a));
    }

    // 로컬 최고 점수 저장 (PlayerPrefs 사용)
    private void SaveHighScores()
    {
        for (int i = 0; i < highScores.Count; i++)
        {
            PlayerPrefs.SetInt($"HighScore{i}", highScores[i]);
        }
        PlayerPrefs.Save();
    }

    [ContextMenu("Reset High Scores")]
    public void ResetHighScores()
    {
        PlayerPrefs.DeleteAll();
        highScores.Clear();
        UpdateUI();
    }
    void OnApplicationQuit()
    {
        SaveCurrentScore();
    }
}
