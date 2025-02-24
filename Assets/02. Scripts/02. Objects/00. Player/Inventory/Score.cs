using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Score", menuName = "Items/Score")]
public class Score : Item
{
    public int score;

    public override void Use(PlayerStats playerStats)
    {
        ScoreManager.Instance.AddScore(score);
    }
}
