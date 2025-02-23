using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPet", menuName = "Pets/PetData")]
public class PetData : ScriptableObject
{
    public string petName;
    public Sprite petSprite;
    public float followSpeed = 3f;
    public float speedBuff = 1f; // �÷��̾�� ������ �ӵ� ����
}
