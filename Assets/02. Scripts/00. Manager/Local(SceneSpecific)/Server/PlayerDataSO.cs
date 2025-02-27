using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDataSO", menuName = "Player Data/PlayerDataSO", order = 1)]
public class PlayerDataSO : ScriptableObject
{
    public string playerName; // �÷��̾� �̸�
    public int highScore;     // �ְ� ����
    public int gold;          // ���� ���
}
