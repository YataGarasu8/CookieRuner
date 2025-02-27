using System;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class NickNameManager : MonoBehaviour
{
    public GameObject nickNamePanel;
    public InputField nickNameInput;  
    public Text nickNameDisplayText; 
    
    private string playerNickName = "Guest"; 

    void Start()
    {
        //PlayerPrefs란? PlayerPrefs 는 Unity에서 제공하는 키-값 쌍을 이용한 데이터 저장 시스템
        playerNickName = PlayerPrefs.GetString("PlayerNickName", "Guest");
        nickNameDisplayText.text = playerNickName;

        nickNamePanel.SetActive(false);

        // 플레이어 닉네임 변경
        GameManager.Instance.PlayerName = playerNickName;
    }

    public void ToggleNickNamePanel()
    {
        // 현재 상태의 반대로 변경
        nickNamePanel.SetActive(!nickNamePanel.activeSelf);
    }

    public void ChangeNickName()
    {
        // 입력 필드가 비어있지 않으면 닉네임 변경
       // String 타입의 매개변수가 null 이거나 값을 입력하지 않았다면 True를 반환하는 프로퍼티
        if (!string.IsNullOrEmpty(nickNameInput.text))
        {
            playerNickName = nickNameInput.text; 
            nickNameDisplayText.text = playerNickName; 

            
            PlayerPrefs.SetString("PlayerNickName", playerNickName);
            PlayerPrefs.Save();

            nickNamePanel.SetActive(false);

            // 플레이어 닉네임 변경
            GameManager.Instance.PlayerName = playerNickName;
        }
    }
}
