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
        if (PlayerStat != null)
        {
            PlayerStats stats = PlayerStat.GetComponent<PlayerStats>() as PlayerStats;
        }
        backGroundBar.sizeDelta = new Vector2(PlayerStat.GetComponent<PlayerStats>().CurrentHealth, 24);
    }
    private void Update()
    {
        HPDown();
    }

    void HPDown()
    {
        if (PlayerStat != null)
        {
            rubTimeHPBar.sizeDelta = new Vector2(PlayerStat.GetComponent<PlayerStats>().CurrentHealth, 0);
            PlayerStat.GetComponent<PlayerStats>().CurrentHealth -= Time.deltaTime;
        }
    }
}
