using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �÷��̾� �����͸� ���� ������ Ŭ���� ����
using System;

[Serializable]
public class PlayerData
{
    public int highScore;  // �ְ� ����
}

// �Ϲ� ������ Ŭ���� - json ������ȭ ����
[Serializable]
public class PlayerDataPlain
{
    public int highScore;
}

// PlayerDataSO must be instantiated using the ScriptableObject.CreateInstance method instead of new PlayerDataSO.
// ScriptableObject�� Newtonsoft.Json���� ���� ������ȭ�Ϸ��� �� �� �߻�
// ScriptableObject�� new �����ڷ� �ν��Ͻ�ȭ�ϸ� �� �ǰ� �ݵ�� ScriptableObject.CreateInstance�� ���� �����ؾ� �ϴµ�,
// Newtonsoft.Json�� ���������� new�� ȣ���Ϸ� �ϱ� ������ �̷� ������ �߻��Ѵٰ� ��
