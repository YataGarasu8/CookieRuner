using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[InitializeOnLoad]
public class GridCoordinateToggle
{
    private static bool showCoordinates = true; // 좌표 표시 여부 저장
    private const string menuPath = "GridTools/Toggle Coordinates %g"; // %g : Ctrl + G 단축키

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
        Menu.SetChecked(menuPath, showCoordinates); // 메뉴에 체크 표시 반영
        return true;
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        if (!showCoordinates) return; // 꺼져 있으면 좌표 표시 안 함

        Event e = Event.current;
        if (e == null || SceneView.lastActiveSceneView == null) return;

        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        Plane plane = new Plane(Vector3.forward, Vector3.zero); // XY 평면

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 worldPos = ray.GetPoint(distance);
            Grid grid = Object.FindObjectOfType<Grid>();

            if (grid != null)
            {
                Vector3Int cellPos = grid.WorldToCell(worldPos);

                Handles.BeginGUI();
                GUI.Label(new Rect(10, 10, 200, 20), $"Grid 좌표: {cellPos}");
                Handles.EndGUI();
            }
        }

        sceneView.Repaint(); // 실시간 갱신
    }
}
