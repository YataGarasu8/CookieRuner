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
            Debug.Log("돈이 부족합니다");
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
            Debug.Log("돈이 부족합니다");
        }
    }

    void UpdateUI()
    {
        cookie1ButtonText.text = isCookie1Purchased ? "구매완료" : "구매";
        cookie2ButtonText.text = isCookie2Purchased ? "구매완료" : "구매";
    }
}

