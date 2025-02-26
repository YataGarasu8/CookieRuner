using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{
    public Button exitButton;

    void Start()
    {
        exitButton = GetComponent<Button>();

       
        exitButton.onClick.AddListener(ExitGame);
    }

    void ExitGame()
    {
        Debug.Log("���� ����");


    }
}
