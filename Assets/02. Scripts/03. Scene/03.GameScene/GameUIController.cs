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

    private void Start()
    {
        PausePanel.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PauseGame();
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
}
