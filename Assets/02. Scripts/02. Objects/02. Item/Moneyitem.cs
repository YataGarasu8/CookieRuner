using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New MoneyItem", menuName = "Items/MoneyItem")]
public class Moneyitem : Item
{
    public float money;

    public override void Use(PlayerStats playerStats)
    {
        
    }
}
