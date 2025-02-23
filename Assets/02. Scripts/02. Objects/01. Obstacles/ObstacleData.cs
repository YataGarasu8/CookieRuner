using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleData", menuName = "Obstacle/Create New Obstacle Data")]
public class ObstacleData : ScriptableObject
{
    public ObstacleActionType type;

    [Header("Static ��ֹ� ����")]
    public Sprite staticSprite;

    [Header("�ִϸ��̼� ��ֹ� ����")]
    public RuntimeAnimatorController animatorController;

    [Header("���� �Ӽ�")]
    public Vector2 size = new Vector2(1, 1);
    public int damage = 1;

    [Header("Flying ��ֹ� ����")]
    public float flyingSpeed = 5f; // ���ƿ��� �ӵ�
}
