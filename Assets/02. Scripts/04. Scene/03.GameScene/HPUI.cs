using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPUI : MonoBehaviour
{
    public PlayerStatsData statsData;
    PlayerStatsData runTimeData;

    private void Awake()
    {
        runTimeData = Instantiate(statsData);
    }
}
