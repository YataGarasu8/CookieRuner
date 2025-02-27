using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharaterSelcet : MonoBehaviour
{
    public TextMeshProUGUI CookieUI1;
    public TextMeshProUGUI CookieUI2;
    public TextMeshProUGUI CookieUI3;

    private void Start()
    {
        CookieUI1 = GameObject.Find("Cookie1").GetComponentInChildren<TextMeshProUGUI>();
        CookieUI2 = GameObject.Find("Cookie2").GetComponentInChildren<TextMeshProUGUI>();
        CookieUI3 = GameObject.Find("Cookie3").GetComponentInChildren<TextMeshProUGUI>();
    }

    public void onClickdefaultCookie()
    {
        GameManager.Instance.charSelect = CharacterSelect.Default.ToString();
        CookieUI1.text = "������";
        CookieUI2.text = "����";
        CookieUI3.text = "����";
        Debug.Log(GameManager.Instance.charSelect);
    }

    public void onClickCookie2()
    {
        GameManager.Instance.charSelect = CharacterSelect.Cookie2.ToString();
        CookieUI1.text = "����";
        CookieUI2.text = "������";
        CookieUI3.text = "����";
        Debug.Log(GameManager.Instance.charSelect);
    }

    public void onClickSorasakiHina()
    {
        GameManager.Instance.charSelect = CharacterSelect.SorasakiHina.ToString();
        CookieUI1.text = "����";
        CookieUI2.text = "����";
        CookieUI3.text = "������";
        Debug.Log(GameManager.Instance.charSelect);
    }
}
