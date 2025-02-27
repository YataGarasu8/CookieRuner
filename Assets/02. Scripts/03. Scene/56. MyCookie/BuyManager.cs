using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyManager : MonoBehaviour
{
    public int cookie1Price = 1500;
    public int cookie2Price = 1500;
    public TextMeshProUGUI cookie1ButtonText;
    public TextMeshProUGUI cookie2ButtonText;
    private GameManager gameManager;
    
    private bool isCookie1Purchased = false;
    private bool isCookie2Purchased = false;

    void Start()
    {
        
    }

    public void BuyCookie1()
    {
        if (isCookie1Purchased) return;

        if (gameManager.Money >= cookie1Price) 
        {
            gameManager.Money -= cookie1Price;
            isCookie1Purchased = true;
            UpdateUI();
        }
        else
        {
            Debug.Log("���� �����մϴ�");
        }
    }

    public void BuyCookie2()
    {
        if (isCookie2Purchased) return;

        if (gameManager.Money >= cookie2Price) 
        {
            gameManager.Money -= cookie2Price;
            isCookie2Purchased = true;
            UpdateUI();
        }
        else
        {
            Debug.Log("���� �����մϴ�");
        }
    }

    void UpdateUI()
    {
        cookie1ButtonText.text = isCookie1Purchased ? "���ſϷ�" : "����";
        cookie2ButtonText.text = isCookie2Purchased ? "���ſϷ�" : "����";
    }
}

