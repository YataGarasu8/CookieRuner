using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour
{
    public Text addGold;
    public TextMeshProUGUI getGold;

    // Start is called before the first frame update
    void Start()
    {
        WriteMoney();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void WriteMoney()
    {
        if (addGold != null)
        {
            addGold.text = GameManager.Instance.Money.ToString();
        }
    }
}
