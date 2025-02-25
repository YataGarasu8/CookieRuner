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
        // 입력 필드가 비어있지 않으면 닉네임 변경
        if (!string.IsNullOrEmpty(nickNameInput.text))
        {
            playerNickName = nickNameInput.text; 
            nickNameDisplayText.text = playerNickName; 

            
            PlayerPrefs.SetString("PlayerNickName", playerNickName);
            PlayerPrefs.Save();
        }
    }
}
