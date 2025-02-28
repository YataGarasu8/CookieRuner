using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePause : MonoBehaviour
{
    public GameObject pauseUI;

    public void onClikePauseButton()
    {
        UIManager.Instance.OpenUI(pauseUI);
        Time.timeScale = 0f;
    }

    public void onClickClosePauseUIButton()
    {
        UIManager.Instance.CloseUI();
        Time.timeScale = 1f;
    }
}
