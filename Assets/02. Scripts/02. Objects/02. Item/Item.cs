using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Item : ScriptableObject
{
    public ItemType Type;

    public string itemName;
    public Sprite icon;
    [Header("���� �Ӽ�")]
    public Vector2 size = new Vector2(1, 1);

    [Header("ȸ�������� ����")]
    public int healthBonus;

    [Header("���ǵ�� ������ ����")]
    public float speedBonus;

    [Header("�Ӵ� ������ ����")]
    public float money;

    [Header("���ھ� ������ ����")]
    public int score;

    public abstract void Use(PlayerStats playerStats);

}
