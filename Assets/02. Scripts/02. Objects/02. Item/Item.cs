using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Item : ScriptableObject
{
    public ItemType Type;

    public string itemName;
    public Sprite icon;
    [Header("공통 속성")]
    public Vector2 size = new Vector2(1, 1);

    [Header("회복아이템 전용")]
    public int healthBonus;

    [Header("스피드업 아이템 전용")]
    public float speedBonus;

    [Header("머니 아이템 전용")]
    public float money;

    [Header("스코어 아이템 전용")]
    public int score;

    public abstract void Use(PlayerStats playerStats);

}
