using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �÷��̾� ���� �����͸� �����ϴ� Ŭ���� (ScriptableObject)
[CreateAssetMenu(fileName = "PlayerStatsData", menuName = "Player/Create Stats Data")]
public class PlayerStatsData : ScriptableObject
{
    // ü�� ���� ������
    [Header("ü��")]
    public int maxHealth = 100;

    // �ӵ� ���� ������
    [Header("�ӵ�")]
    [Range(1f, 10f)] public float baseSpeed = 1f;

    // ũ�� ���� ������
    [Header("ũ��")]
    public float baseScale = 1f;
    public float maxScale = 2f;
    public float scaleIncreaseAmount = 0.2f;
    public float scaleDuration = 5f;
    public float scaleChangeSpeed = 2f;
}
