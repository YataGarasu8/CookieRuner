using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[InitializeOnLoad]
public class GridCoordinateToggle
{
    private static bool showCoordinates = true; // ��ǥ ǥ�� ���� ����
    private const string menuPath = "GridTools/Toggle Coordinates %g"; // %g : Ctrl + G ����Ű

    static GridCoordinateToggle()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    [MenuItem(menuPath)]
    public static void ToggleCoordinates()
    {
        showCoordinates = !showCoordinates;
        SceneView.RepaintAll();
    }

    [MenuItem(menuPath, true)]
    public static bool ToggleCoordinatesValidate()
    {
        Menu.SetChecked(menuPath, showCoordinates); // �޴��� üũ ǥ�� �ݿ�
        return true;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        if (!showCoordinates) return; // ���� ������ ��ǥ ǥ�� �� ��

        Event e = Event.current;
        if (e == null || SceneView.lastActiveSceneView == null) return;

        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        Plane plane = new Plane(Vector3.forward, Vector3.zero); // XY ���

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 worldPos = ray.GetPoint(distance);
            Grid grid = Object.FindObjectOfType<Grid>();

            if (grid != null)
            {
                Vector3Int cellPos = grid.WorldToCell(worldPos);

                Handles.BeginGUI();
                GUI.Label(new Rect(10, 10, 200, 20), $"Grid ��ǥ: {cellPos}");
                Handles.EndGUI();
            }
        }

        sceneView.Repaint(); // �ǽð� ����
    }
}
