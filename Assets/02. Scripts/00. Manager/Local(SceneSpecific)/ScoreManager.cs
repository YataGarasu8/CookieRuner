using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("UI Elements")]
    public Text scoreText; // 현재 점수 표시
    public Text highScoresText; // 역대 점수 표시

    [Header("Settings")]
    public int maxHighScores = 10; // 최대 순위 개수

    private int currentScore = 0;
    private List<int> highScores = new List<int>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        LoadHighScores();
        UpdateUI();
    }

    // 점수 추가
    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateUI();
    }

    // 게임 종료 시 점수 저장
    public void SaveCurrentScore()
    {
        highScores.Add(currentScore);
        highScores.Sort((a, b) => b.CompareTo(a)); // 내림차순 정렬

        if (highScores.Count > maxHighScores)
            highScores.RemoveRange(maxHighScores, highScores.Count - maxHighScores);

        SaveHighScores();
        UpdateUI();
    }

    // UI 업데이트
    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"현재 점수: {currentScore}";

        if (highScoresText != null)
        {
            highScoresText.text = "?? 역대 최고 점수:\n";
            for (int i = 0; i < highScores.Count; i++)
            {
                highScoresText.text += $"{i + 1}. {highScores[i]}점\n";
            }
        }
    }

    // 최고 점수 로드
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

    // 최고 점수 저장
    private void SaveHighScores()
    {
        for (int i = 0; i < highScores.Count; i++)
        {
            PlayerPrefs.SetInt($"HighScore{i}", highScores[i]);
        }
        PlayerPrefs.Save();
    }

    // 점수 초기화 (디버그용)
    [ContextMenu("Reset High Scores")]
    public void ResetHighScores()
    {
        PlayerPrefs.DeleteAll();
        highScores.Clear();
        UpdateUI();
    }
}
