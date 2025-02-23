using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어 스탯 데이터를 보관하는 클래스 (ScriptableObject)
[CreateAssetMenu(fileName = "PlayerStatsData", menuName = "Player/Create Stats Data")]
public class PlayerStatsData : ScriptableObject
{
    // 체력 관련 데이터
    [Header("체력")]
    public int maxHealth = 100;

    // 속도 관련 데이터
    [Header("속도")]
    [Range(1f, 10f)] public float baseSpeed = 1f;

    // 크기 관련 데이터
    [Header("크기")]
    public float baseScale = 1f;
    public float maxScale = 2f;
    public float scaleIncreaseAmount = 0.2f;
    public float scaleDuration = 5f;
    public float scaleChangeSpeed = 2f;
}
