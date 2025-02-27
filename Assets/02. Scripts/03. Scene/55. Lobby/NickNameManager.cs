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
        //PlayerPrefs��? PlayerPrefs �� Unity���� �����ϴ� Ű-�� ���� �̿��� ������ ���� �ý���
        playerNickName = PlayerPrefs.GetString("PlayerNickName", "Guest");
        nickNameDisplayText.text = playerNickName;

        nickNamePanel.SetActive(false);

        // �÷��̾� �г��� ����
        GameManager.Instance.PlayerName = playerNickName;
    }

    public void ToggleNickNamePanel()
    {
        // ���� ������ �ݴ�� ����
        nickNamePanel.SetActive(!nickNamePanel.activeSelf);
    }

    public void ChangeNickName()
    {
        // �Է� �ʵ尡 ������� ������ �г��� ����
       // String Ÿ���� �Ű������� null �̰ų� ���� �Է����� �ʾҴٸ� True�� ��ȯ�ϴ� ������Ƽ
        if (!string.IsNullOrEmpty(nickNameInput.text))
        {
            playerNickName = nickNameInput.text; 
            nickNameDisplayText.text = playerNickName; 

            
            PlayerPrefs.SetString("PlayerNickName", playerNickName);
            PlayerPrefs.Save();

            nickNamePanel.SetActive(false);

            // �÷��̾� �г��� ����
            GameManager.Instance.PlayerName = playerNickName;
        }
    }
}
