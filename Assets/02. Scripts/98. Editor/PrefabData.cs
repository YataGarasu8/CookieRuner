using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPrefabData", menuName = "Prefab Data")]
public class PrefabData : ScriptableObject
{
    public string prefabName;       // 프리팹 이름
    public GameObject prefab;       // 연결된 프리팹
}
