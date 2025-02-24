using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Treasure", menuName = "Items/Treasure")]
public class SpeedUPItem : Item
{
    public float speedBonus;

    public override void Use(PlayerStats playerStats)
    {
        playerStats.ModifySpeed(speedBonus);
        Debug.Log($"Used {itemName}: +{speedBonus} Speed");
    }
}
