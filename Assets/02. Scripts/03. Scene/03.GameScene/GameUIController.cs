using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUIController : MonoBehaviour
{
    public TextMeshProUGUI CoinCountText;
    public GameObject PausePanel;
    public GameObject EndPanel;
    public GameObject Canvas;



    private void Awake()
    {
        PausePanel.gameObject.SetActive(false);
        EndPanel.gameObject.SetActive(false);
    }
    private void Start()
    {
        Canvas.gameObject.SetActive(true);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PauseGame();
        }

        CoinCountText.text = GameManager.Instance.Money.ToString();

        if(GameManager.Instance.IsGameOver == true)
        {
            EndPanel.gameObject.SetActive(true);
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
        Canvas.gameObject.SetActive(false);
        EndPanel.gameObject.SetActive(false);
        SceneManager.LoadScene("LobbyScene");
        Time.timeScale = 1f;
    }

}
