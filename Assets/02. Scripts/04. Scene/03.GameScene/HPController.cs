using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPCotroller : MonoBehaviour
{
    public GameObject PlayerStatus;

    public RectTransform rubTimeHPBar;

    float runTimeHP;

    private void Awake()
    {
        //runTimeData = Instantiate(statsData);   
    }
    private void Start()
    {
        //runTimeHP = runTimeData.maxHealth;
    }
    private void Update()
    {
        HPDown();
    }

    void HPDown()
    {
        rubTimeHPBar.sizeDelta = new Vector2(runTimeHP, 0);
        runTimeHP -= Time.deltaTime;
    }
}
