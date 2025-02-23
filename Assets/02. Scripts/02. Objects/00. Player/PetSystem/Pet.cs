using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(PetController))]
public class Pet : MonoBehaviour
{
    public PetData petData;
    private PlayerStats playerStats;

    void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        ApplyBuff();
    }

    void ApplyBuff()
    {
        if (playerStats != null && petData != null)
        {
            playerStats.ModifySpeed(petData.speedBuff);
            Debug.Log($"{petData.petName} applied speed buff: +{petData.speedBuff}");
        }
    }
}
