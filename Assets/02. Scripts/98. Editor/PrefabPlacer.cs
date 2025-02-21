using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PrefabPlacer : EditorWindow
{
    public GameObject prefab;  // ������ ���� �ʵ�

    [MenuItem("Tools/Prefab Placer")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<PrefabPlacer>("Prefab Placer");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Prefab Placer", EditorStyles.boldLabel);

        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

        if (prefab != null && GUILayout.Button("Place Prefab"))
        {
            PlacePrefab();
        }
    }

    private void PlacePrefab()
    {
        Vector3 spawnPosition = SceneView.lastActiveSceneView.camera.transform.position + Vector3.forward * 10;
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.transform.position = spawnPosition;

        Debug.Log($"[Prefab Placer] '{prefab.name}' ��ġ �Ϸ� at {spawnPosition}");
    }
}
