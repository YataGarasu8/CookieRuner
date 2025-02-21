using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("UI Elements")]
    public Text scoreText; // ���� ���� ǥ��
    public Text highScoresText; // ���� ���� ǥ��

    [Header("Settings")]
    public int maxHighScores = 10; // �ִ� ���� ����

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

    // ���� �߰�
    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateUI();
    }

    // ���� ���� �� ���� ����
    public void SaveCurrentScore()
    {
        highScores.Add(currentScore);
        highScores.Sort((a, b) => b.CompareTo(a)); // �������� ����

        if (highScores.Count > maxHighScores)
            highScores.RemoveRange(maxHighScores, highScores.Count - maxHighScores);

        SaveHighScores();
        UpdateUI();
    }

    // UI ������Ʈ
    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"���� ����: {currentScore}";

        if (highScoresText != null)
        {
            highScoresText.text = "?? ���� �ְ� ����:\n";
            for (int i = 0; i < highScores.Count; i++)
            {
                highScoresText.text += $"{i + 1}. {highScores[i]}��\n";
            }
        }
    }

    // �ְ� ���� �ε�
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

    // �ְ� ���� ����
    private void SaveHighScores()
    {
        for (int i = 0; i < highScores.Count; i++)
        {
            PlayerPrefs.SetInt($"HighScore{i}", highScores[i]);
        }
        PlayerPrefs.Save();
    }

    // ���� �ʱ�ȭ (����׿�)
    [ContextMenu("Reset High Scores")]
    public void ResetHighScores()
    {
        PlayerPrefs.DeleteAll();
        highScores.Clear();
        UpdateUI();
    }
}
