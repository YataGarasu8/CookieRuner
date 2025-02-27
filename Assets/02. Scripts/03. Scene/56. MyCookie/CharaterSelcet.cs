using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharaterSelcet : MonoBehaviour
{
    TextMeshProUGUI CookieUI1;
    TextMeshProUGUI CookieUI2;
    TextMeshProUGUI CookieUI3;
    TextMeshProUGUI CookieName2;
    TextMeshProUGUI CookieName3;

    Image Santa;
    Image Hina;

    public Sprite HinaStaned;
    public Sprite SantaStaned;

    private void Start()
    {
        GetFind();
        ChangeImage();
    }
    private void Update()
    {
        
    }

    public void onClickdefaultCookie()
    {
        GameManager.Instance.charSelect = CharacterSelect.Default;
        CookieUI1.text = "ÀåÂøÁß";
        CookieUI2.text = "º¯°æ";
        CookieUI3.text = "º¯°æ";
        Debug.Log(GameManager.Instance.charSelect);
    }

    public void onClickCookie2()
    {
        if (GameManager.Instance.SantaGet)
        {
            GameManager.Instance.charSelect = CharacterSelect.Cookie2;
            CookieUI1.text = "º¯°æ";
            CookieUI2.text = "ÀåÂøÁß";
            CookieUI3.text = "º¯°æ";
            Debug.Log(GameManager.Instance.charSelect);
        }
    }

    public void onClickSorasakiHina()
    {
        if (GameManager.Instance.HinaGet)
        {
            GameManager.Instance.charSelect = CharacterSelect.SorasakiHina;
            CookieUI1.text = "º¯°æ";
            CookieUI2.text = "º¯°æ";
            CookieUI3.text = "ÀåÂøÁß";
            Debug.Log(GameManager.Instance.charSelect);
        }
    }
    public void ChangeImage()
    {
        if (GameManager.Instance.SantaGet)
        {  
            Santa.sprite = SantaStaned;
            Santa.color = new Color(1, 1, 1, 1);
            CookieName2.text = "»êÅ¸¸À ÄíÅ°";
        }
        if (GameManager.Instance.HinaGet)
        { 
            Hina.sprite = HinaStaned;
            Hina.color = new Color(1, 1, 1, 1);
            CookieName3.text = "È÷³ª¸À ÄíÅ°";
        }
    }
    public void GetFind()
    {
        CookieUI1 = GameObject.Find("Cookie1").GetComponentInChildren<TextMeshProUGUI>();
        CookieUI2 = GameObject.Find("Cookie2").GetComponentInChildren<TextMeshProUGUI>();
        CookieUI3 = GameObject.Find("Cookie3").GetComponentInChildren<TextMeshProUGUI>();
        CookieName2 = GameObject.Find("CookieName2").GetComponentInChildren<TextMeshProUGUI>();
        CookieName3 = GameObject.Find("CookieName3").GetComponentInChildren<TextMeshProUGUI>();
        Santa = GameObject.Find("Cookie2").GetComponent<Image>();
        Hina = GameObject.Find("Cookie3").GetComponent<Image>();
    }
}
