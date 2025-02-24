using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    public TextMeshProUGUI CoinCountText;
    public GameObject PausePanel;

    int coinCount;

    private void Update()
    {
        PauseGame();
    }

    public void PauseGame()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PausePanel.gameObject.SetActive(true);
            PausePanelButton();
            Time.timeScale = 0f;
        }
    }
    public void PausePanelButton()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            //메인메뉴 돌아가기
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            PausePanel.gameObject.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}
