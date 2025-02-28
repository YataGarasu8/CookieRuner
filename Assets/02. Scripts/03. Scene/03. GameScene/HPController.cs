using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPCotroller : MonoBehaviour
{
    public GameObject PlayerStat;

    public RectTransform rubTimeHPBar;
    public RectTransform backGroundBar;


    private void Start()
    {
        rubTimeHPBar.sizeDelta = new Vector2(PlayerStat.GetComponent<PlayerStats>().CurrentHealth * 2, 50);
        if (PlayerStat != null)
        {
            PlayerStats stats = PlayerStat.GetComponent<PlayerStats>() as PlayerStats;
        }
        backGroundBar.sizeDelta = new Vector2(PlayerStat.GetComponent<PlayerStats>().CurrentHealth * 2, 50);
    }
    private void Update()
    {
        HPDown();
    }

    void HPDown()
    {
        if (PlayerStat != null)
        {
            rubTimeHPBar.sizeDelta = new Vector2(PlayerStat.GetComponent<PlayerStats>().CurrentHealth * 2, 50);
            PlayerStat.GetComponent<PlayerStats>().CurrentHealth -= Time.deltaTime;
        }
    }
}
