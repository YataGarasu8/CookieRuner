using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{
    public Button exitButton;



    public void ExitGame()
    {
        Debug.Log("게임 종료");

        UnityEditor.EditorApplication.isPlaying = false;//유니티 플레이 종료
        Application.Quit();//빌드 된 게임 종료
    }
}
