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

    public abstract void Use(PlayerStats playerStats);

}
