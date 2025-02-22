using UnityEditor;
using UnityEngine;
using System.IO;

public static class FolderUtility
{
    /// ���� ��ο��� ������ �������� ������ �ڵ����� ����
    public static void CreateFolderForFileIfNotExists(string filePath)
    {
        string folderPath = Path.GetDirectoryName(filePath); // ���� ��� ����

        if (string.IsNullOrEmpty(folderPath))
        {
            Debug.LogError($"�߸��� ���� ���: {filePath}");
            return;
        }

        if (AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.Log($"���� ����: {folderPath}");
            return;
        }

        // Assets �������� ���� ����
        string[] pathParts = folderPath.Split("\\");
        string currentPath = pathParts[0];  // �Ϲ������� "Assets"

        for (int i = 1; i < pathParts.Length; i++)
        {
            string nextPath = $"{currentPath}/{pathParts[i]}";
            if (!AssetDatabase.IsValidFolder(nextPath))
            {
                AssetDatabase.CreateFolder(currentPath, pathParts[i]);
                Debug.Log($" ���� ����: {nextPath}");
            }
            currentPath = nextPath;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"���� ���� �Ϸ�: {folderPath}");
    }
}
