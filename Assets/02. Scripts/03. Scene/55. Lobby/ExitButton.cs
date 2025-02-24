using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{
    public Button exitButton;  

    void Start()
    {            
            exitButton.onClick.AddListener(ExitGame);    
    }

    void ExitGame()
    {
        Debug.Log("게임 종료");        
    }
}