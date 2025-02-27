using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyManager : MonoBehaviour
{
    int cookie1Price = 1500;
    int cookie2Price = 1500;
    public TextMeshProUGUI cookie1ButtonText;
    public TextMeshProUGUI cookie2ButtonText;

    void Start()
    {

    }
    private void Update()
    {
        Sale();
    }

    public void BuyCookie1()
    {
        if (GameManager.Instance.Money >= cookie1Price)
        {
            if (!GameManager.Instance.HinaGet)
            {
                GameManager.Instance.Money -= cookie1Price;
                GameManager.Instance.HinaGet = true;
            }
        }
        else
        {
            Debug.Log("돈이 부족합니다");
        }
    }
    public void BuyCookie2()
    {
        if (GameManager.Instance.Money >= cookie2Price)
        {
            if (!GameManager.Instance.SantaGet)
            {
                GameManager.Instance.Money -= cookie2Price;
                GameManager.Instance.SantaGet = true;
            }
        }
        else
        {
            Debug.Log("돈이 부족합니다");
        }
    }
    void Sale()
    {
        if (GameManager.Instance.HinaGet == true)
        { cookie1ButtonText.text = "구매 완료"; }
        if (GameManager.Instance.SantaGet == true)
        { cookie2ButtonText.text = "구매 완료"; }
    }
}

