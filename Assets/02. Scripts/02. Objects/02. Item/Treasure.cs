using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "New Treasure", menuName = "Items/Treasure")]
public class Treasure : Item
{
    public float speedBonus;
    public int healthBonus;

    /*public override void Use(PlayerStats playerStats)
    {
        playerStats.ModifySpeed(speedBonus);
        playerStats.Heal(healthBonus);
        Debug.Log($"Used {itemName}: +{speedBonus} Speed, +{healthBonus} Health");
    }*/
}
