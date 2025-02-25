using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDataSO", menuName = "Player Data/PlayerDataSO", order = 1)]
public class PlayerDataSO : ScriptableObject
{
    public int highScore;  // 최고 점수
}
