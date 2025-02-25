using UnityEngine;
using UnityEngine.UI;

public class NickNameManager : MonoBehaviour
{
    public InputField nickNameInput;  
    public Text nickNameDisplayText; 

    private string playerNickName = "Guest"; 

    void Start()
    {
        
        playerNickName = PlayerPrefs.GetString("PlayerNickName", "Guest");
        nickNameDisplayText.text = playerNickName;
    }

    public void ChangeNickName()
    {
        // �Է� �ʵ尡 ������� ������ �г��� ����
        if (!string.IsNullOrEmpty(nickNameInput.text))
        {
            playerNickName = nickNameInput.text; 
            nickNameDisplayText.text = playerNickName; 

            
            PlayerPrefs.SetString("PlayerNickName", playerNickName);
            PlayerPrefs.Save();
        }
    }
}
