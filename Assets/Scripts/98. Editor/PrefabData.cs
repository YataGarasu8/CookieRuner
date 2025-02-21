using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPrefabData", menuName = "Prefab Data")]
public class PrefabData : ScriptableObject
{
    public string prefabName;       // ������ �̸�
    public GameObject prefab;       // ����� ������
}
