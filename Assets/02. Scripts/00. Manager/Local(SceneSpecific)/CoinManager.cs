using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour
{
    public Text addGold;
    

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.CalculateMoney();
        WriteMoney();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void WriteMoney()
    {
        if (addGold != null)
        {
            addGold.text = GameManager.Instance.playerMoney.ToString();
        }
    }
}
