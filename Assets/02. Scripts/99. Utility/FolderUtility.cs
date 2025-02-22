using UnityEditor;
using UnityEngine;
using System.IO;

public static class FolderUtility
{
    /// 파일 경로에서 폴더가 존재하지 않으면 자동으로 생성
    public static void CreateFolderForFileIfNotExists(string filePath)
    {
        string folderPath = Path.GetDirectoryName(filePath); // 폴더 경로 추출

        if (string.IsNullOrEmpty(folderPath))
        {
            Debug.LogError($"잘못된 파일 경로: {filePath}");
            return;
        }

        if (AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.Log($"폴더 존재: {folderPath}");
            return;
        }

        // Assets 기준으로 폴더 생성
        string[] pathParts = folderPath.Split("\\");
        string currentPath = pathParts[0];  // 일반적으로 "Assets"

        for (int i = 1; i < pathParts.Length; i++)
        {
            string nextPath = $"{currentPath}/{pathParts[i]}";
            if (!AssetDatabase.IsValidFolder(nextPath))
            {
                AssetDatabase.CreateFolder(currentPath, pathParts[i]);
                Debug.Log($" 폴더 생성: {nextPath}");
            }
            currentPath = nextPath;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"폴더 생성 완료: {folderPath}");
    }
}
