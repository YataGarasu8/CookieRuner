using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.CloudSave;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;          // ���� ���� ǥ��
    public TextMeshProUGUI highScoresText;     // ���� �ְ� ���� ǥ��

    [Header("Settings")]
    public int maxHighScores = 10;  // �ִ� ���� ����

    private int currentScore = 0;   // ���� ����
    private List<int> highScores = new List<int>(); // ���� �ְ� ���� ���

    [Header("Game UI Controller")]
    public GameUIController gameUIController;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadHighScores();
        //UpdateUI();


        // ���ο� ���� �ε�� �� UI ���� �缳��
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject gameUI = GameObject.Find("GameUI");
        if (gameUI != null)
        {
            // GameUI ���� Canvas ������Ʈ�� ã���ϴ�.
            Transform canvasTransform = gameUI.transform.Find("Canvas");
            if (canvasTransform != null)
            {
                // Canvas ������ ScoreText ������Ʈ�� ã���ϴ�.
                Transform scoreTextTransform = canvasTransform.Find("ScoreText");
                if (scoreTextTransform != null)
                {
                    scoreText = scoreTextTransform.GetComponent<TextMeshProUGUI>();
                    scoreText.text = $"���� ����: {currentScore}";
                    Debug.Log("ScoreText ������Ʈ�� ���������� ã�ҽ��ϴ�.");
                }
                else
                {
                    Debug.LogError("Canvas �������� ScoreText ������Ʈ�� ã�� ���߽��ϴ�.");
                }
            }
            else
            {
                Debug.LogError("GameUI �������� Canvas ������Ʈ�� ã�� ���߽��ϴ�.");
            }
        }
        else
        {
            Debug.LogError("DontDestroyOnLoad�� �ִ� GameUI ������Ʈ�� ã�� ���߽��ϴ�.");
        }

        GameObject rankingBoard = GameObject.Find("RankingBoard");
        if (rankingBoard != null)
        {
            Transform highScoreTransform = rankingBoard.transform.Find("HighScoreText");
            if (highScoreTransform != null)
            {
                highScoresText = highScoreTransform.GetComponent<TextMeshProUGUI>();
                // targetText ���
            }
            else
            {
                Debug.LogError("HighScoreText ������Ʈ�� ã�� �� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogError("RankingBoard ������Ʈ�� ã�� �� �����ϴ�.");
        }
    }

    public void ResetCurrentScore()
    {
        currentScore = 0;
    }

    // ���� ���� ������ �߰��ϴ� �Լ�
    public void AddScore(int amount)
    {
        currentScore += amount;
        //UpdateUI();
        Debug.Log($"���� ����: {currentScore}");

        if (scoreText != null)
            scoreText.text = $"���� ����: {currentScore}";
    }
    //void Update()
    //{
    //    // Q Ű�� ������ ��� SaveCurrentScore() ����
    //    if (Input.GetKeyDown(KeyCode.Q))
    //    {
    //        Debug.Log("���� ����");
    //        SaveCurrentScore();
    //    }
    //}

    // ���� ���� �� ȣ��Ǵ� �Լ� (�� ���� Ŭ���忡 ����)
    public void SaveCurrentScore()
    {
        // ���� �ְ� ���� ��� ������Ʈ
        highScores.Add(currentScore);
        highScores.Sort((a, b) => b.CompareTo(a)); // �������� ����
        if (highScores.Count > maxHighScores)
            highScores.RemoveRange(maxHighScores, highScores.Count - maxHighScores);
        SaveHighScores();
        UpdateUI();

        // PlayerDataManager�� ScriptableObject�� ���Ͽ� �ְ� ���� ����
        if (PlayerDataManager.Instance != null && PlayerDataManager.Instance.playerDataSO != null)
        {
            PlayerDataManager.Instance.playerDataSO.highScore = Mathf.Max(currentScore, PlayerDataManager.Instance.playerDataSO.highScore);
            Debug.Log("Ŭ���忡 ������ �ְ� ������ ������Ʈ�մϴ�: " + currentScore);
            // ���� ���� �� �� ���� Ŭ���忡 ����
#pragma warning disable CS4014
            PlayerDataManager.Instance.SavePlayerDataAsync();
#pragma warning restore CS4014
        }
        else
        {
            Debug.LogError("PlayerDataManager �Ǵ� playerDataSO�� null�Դϴ�!");
        }

        gameUIController.GameOver();
    }

    // UI ������Ʈ �Լ�
    public void UpdateUI()
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
            proUGUI.text += $"{score.Rank+1}��  ���� : {score.Score}\n";
            Debug.Log($"����: {score.Rank}, �÷��̾� ID: {score.PlayerId}, ����: {score.Score}");
        }

        highScoresText.text = "";
        highScoresText.text += proUGUI.text;

        if (scoreText != null)
            scoreText.text = $"���� ����: {currentScore}";

        //if (highScoresText != null)
        //{
        //    highScoresText.text = "���� �ְ� ����:\n";
        //    for (int i = 0; i < highScores.Count; i++)
        //    {
        //        highScoresText.text += $"{i + 1}. {highScores[i]}��\n";
        //    }
        //}
    }

    // ���� �ְ� ���� �ε� (PlayerPrefs ���)
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

    // ���� �ְ� ���� ���� (PlayerPrefs ���)
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
