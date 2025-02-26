using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using TMPro;


public class MentManager : MonoBehaviour
{
    [SerializeField] List<string> CookieMent;  // ���� ��� ����Ʈ

    public GameObject PlayerMent;  // ��ǳ�� UI ������Ʈ
    public TextMeshProUGUI PlayerTalk;  // ��ǳ�� �ؽ�Ʈ ������Ʈ

    public Button playerButton;  // Player ĳ���� ��ư

    void Start()
    {
        // ��ǳ�� �ʱ� ���´� ��Ȱ��ȭ
        PlayerMent.SetActive(false);
        if (CookieMent == null)
        {
            CookieMent = new List<string>();
        }

        if (CookieMent.Count == 0) // ��� ���� �� �⺻ ��� �߰�
        {
            CookieMent.Add("���� ���� ������ �Դϴ�!");
            CookieMent.Add("�ȳ��ϼ���~");
            CookieMent.Add("���� �Ϸ� �Ǽ���!");
        }

        // ��ư Ŭ�� �̺�Ʈ ����
        playerButton.onClick.AddListener(LobbyPlayerClick);
    }

    // ��ǳ���� ���� ��縦 ���
    public void LobbyPlayerClick()
    {
        string playerMent = CookieMent[Random.Range(0, CookieMent.Count)];  // ���� ��� ����
        PlayerTalk.text = playerMent;  // �ؽ�Ʈ ������Ʈ

        PlayerMent.SetActive(true);  // ��ǳ�� ǥ��

        Invoke("EraseMentBox", 10f);  // 3.3�� �� ��ǳ�� �����
    }

    // ��ǳ�� �����
    public void EraseMentBox()
    {
        PlayerMent.SetActive(false);  // ��ǳ�� ��Ȱ��ȭ
    }

}
