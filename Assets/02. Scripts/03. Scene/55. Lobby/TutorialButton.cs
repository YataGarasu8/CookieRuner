using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialButton : MonoBehaviour
{

    public void LoadTutorialScene()
    {
        if (GameManager.Instance.isTutorialPlay == true)
            return;
        else
        {
            SceneManager.LoadScene("TutorialScene");  //
            GameManager.Instance.isTutorialPlay = true;
        }
        
    }
}
