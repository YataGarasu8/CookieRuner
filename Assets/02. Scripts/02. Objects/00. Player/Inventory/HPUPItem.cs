using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New HPUPItem", menuName = "Items/HPUPItem")]
public class HPUPItem : Item
{
    public int healthBonus;

    public override void Use(PlayerStats playerStats)
    {
        playerStats.Heal(healthBonus);
        Debug.Log($"Used {itemName}: + {healthBonus} Health");
    }
}
