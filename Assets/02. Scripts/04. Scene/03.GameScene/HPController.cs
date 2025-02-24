using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HPCotroller : MonoBehaviour
{
    public PlayerStatsData statsData;
    PlayerStatsData runTimeData;

    public Slider Slider;
    Image fillArea;


    private void Awake()
    {
        runTimeData = Instantiate(statsData);
    }
}
