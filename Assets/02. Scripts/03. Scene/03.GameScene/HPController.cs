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
        backGroundBar.sizeDelta = new Vector2(PlayerStat.GetComponent<PlayerStats>().currentHealth, 24);
    }
    private void Update()
    {
        HPDown();
    }

    void HPDown()
    {
        rubTimeHPBar.sizeDelta = new Vector2(PlayerStat.GetComponent<PlayerStats>().currentHealth, 0);
        PlayerStat.GetComponent<PlayerStats>().currentHealth -= Time.deltaTime;
    }
}
