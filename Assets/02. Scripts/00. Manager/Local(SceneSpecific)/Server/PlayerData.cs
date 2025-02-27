using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �÷��̾� �����͸� ���� ������ Ŭ���� ����
using System;

[Serializable]
public class PlayerData
{
    public string playerName;   // �÷��̾� �̸�
    public int highScore;       // �ְ� ����
    public int gold;            // ���
}


// JSON ����ȭ/������ȭ�� ���� ������ Ŭ����
[Serializable]
public class PlayerDataPlain
{
    public string playerName;
    public int highScore;
    public int gold;
}


// PlayerDataSO must be instantiated using the ScriptableObject.CreateInstance method instead of new PlayerDataSO.
// ScriptableObject�� Newtonsoft.Json���� ���� ������ȭ�Ϸ��� �� �� �߻�
// ScriptableObject�� new �����ڷ� �ν��Ͻ�ȭ�ϸ� �� �ǰ� �ݵ�� ScriptableObject.CreateInstance�� ���� �����ؾ� �ϴµ�,
// Newtonsoft.Json�� ���������� new�� ȣ���Ϸ� �ϱ� ������ �̷� ������ �߻��Ѵٰ� ��
