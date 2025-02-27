using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDataSO", menuName = "Player Data/PlayerDataSO", order = 1)]
public class PlayerDataSO : ScriptableObject
{
    public string playerName; // 플레이어 이름
    public int highScore;     // 최고 점수
    public int gold;          // 보유 골드
}
