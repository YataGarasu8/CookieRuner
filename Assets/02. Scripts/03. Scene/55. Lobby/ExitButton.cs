using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{
    public Button exitButton;



    public void ExitGame()
    {
        Debug.Log("���� ����");

        UnityEditor.EditorApplication.isPlaying = false;//����Ƽ �÷��� ����
        Application.Quit();//���� �� ���� ����
    }
}
