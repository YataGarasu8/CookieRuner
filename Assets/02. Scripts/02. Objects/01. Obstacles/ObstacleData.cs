using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleData", menuName = "Obstacle/Create New Obstacle Data")]
public class ObstacleData : ScriptableObject
{
    public ObstacleActionType type;

    [Header("Static 장애물 전용")]
    public Sprite staticSprite;

    [Header("애니메이션 장애물 전용")]
    public RuntimeAnimatorController animatorController;

    [Header("공통 속성")]
    public Vector2 size = new Vector2(1, 1);
    public int damage = 1;

    [Header("Flying 장애물 전용")]
    public float flyingSpeed = 5f; // 날아오는 속도
}
