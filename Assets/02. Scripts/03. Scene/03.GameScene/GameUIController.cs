using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameUIController : MonoBehaviour
{
    public TextMeshProUGUI CoinCountText;
    public GameObject PausePanel;
    public GameObject EndPanel;
    public GameObject RankingBoard;
    public GameObject Canvas;

    bool isMove = false;
    int startMoney;

    private void Awake()
    {
        PausePanel.gameObject.SetActive(false);
        EndPanel.gameObject.SetActive(false);
        RankingBoard.gameObject.SetActive(false);
    }
    private void Start()
    {
        Canvas.gameObject.SetActive(true);
        CoinSet();


    }
    private void Update()
    {
        CoinCalCulate();
        if (Input.GetKeyDown(KeyCode.P))
        {
            PauseGame();
        }
        if (GameManager.Instance.IsGameOver == true)
        {
            GameOver();
        }
    }

    public void PauseGame()
    {
        PausePanel.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }
    public void ReturnGame()
    {
        PausePanel.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
    public void OutGame()
    {
        PausePanel.gameObject.SetActive(false);
        EndPanel.gameObject.SetActive(false);
        RankingBoard.gameObject.SetActive(false);
        Canvas.gameObject.SetActive(false);
        SoundManager.Instance.StopBGM();
        SceneManager.LoadScene("LobbyScene");
        Time.timeScale = 1f;
    }
    public void GameOver()
    {
        EndPanel.gameObject.SetActive(true);
        Invoke("SetRankingBoard", 2.2f);
    }
    public void SetRankingBoard()
    {
        if (!isMove)
        {
            if (!RankingBoard.activeSelf)
            {
                RankingBoard.gameObject.SetActive(true);
                RankingBoard.transform.DOLocalMoveY(0, 0.5f).SetEase(Ease.OutExpo).OnComplete
                    (
                    () => { isMove = false; }
                    );
            }
        }
    }
    public void CoinSet()
    {
        startMoney = GameManager.Instance.Money;
    }
    public void CoinCalCulate()
    {
        GameManager.Instance.getMoney = GameManager.Instance.Money - startMoney;
        CoinCountText.text = GameManager.Instance.getMoney.ToString();
    }
}
