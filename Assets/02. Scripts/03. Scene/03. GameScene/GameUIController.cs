using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Threading.Tasks;

public class GameUIController : MonoBehaviour
{
    public TextMeshProUGUI CoinCountText;
    public GameObject PausePanel;
    public GameObject RankingBoard;
    public GameObject Canvas;

    bool isMove = false;
    int startMoney;
    public static GameUIController Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        RankingBoard.gameObject.SetActive(false);
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 로드 시마다 실행할 초기화 코드
        if (scene.name == "GameScene")
        {
            Canvas.gameObject.SetActive(true);
            CoinSet();
        }
    }

    private void Update()
    {
        CoinCalCulate();
        if (Input.GetKeyDown(KeyCode.P))
        {
            PauseGame();
        }
        //if (GameManager.Instance.IsGameOver == true)
        //{
        //    GameOver();
        //    GameManager.Instance.IsGameOver = false;
        //}
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
    public async void OutGame()
    {
        await ScoreManager.Instance.SaveCurrentScore();

        RankingBoard.gameObject.SetActive(false);
        PausePanel.gameObject.SetActive(false);
        Canvas.gameObject.SetActive(false);
        SoundManager.Instance.StopBGM();
        SceneManager.LoadScene("LobbyScene");
        ScoreManager.Instance.ResetCurrentScore();
        Time.timeScale = 1f;
    }
    public void GameOver()
    {
        Canvas.gameObject.SetActive(true);;
        RankingBoard.gameObject.SetActive(true);
        //Invoke("SetRankingBoard", 0);
    }
    //public void SetRankingBoard()
    //{
    //    if (!isMove)
    //    {
    //        if (!RankingBoard.activeSelf)
    //        {
    //            RankingBoard.gameObject.SetActive(true);
    //            RankingBoard.transform.DOLocalMoveY(0, 0.5f).SetEase(Ease.OutExpo).OnComplete
    //                (
    //                () => { isMove = false; }
    //                );
    //        }
    //    }
    //}
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
