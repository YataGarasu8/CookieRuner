// ==========================
// SpriteSheetCutter.cs (업데이트: 특정 좌표와 이름으로 잘라내기, 이모티콘 제거)
// ==========================

using UnityEditor;
using UnityEngine;
using System.IO;

public class SpriteSheetCutter : MonoBehaviour
{
    [Header("스프라이트 시트 설정")]
    public Texture2D spriteSheet; // 자를 스프라이트 시트 이미지
    public int spriteWidth = 264;  // 각 스프라이트의 가로 크기
    public int spriteHeight = 264; // 각 스프라이트의 세로 크기

    [Header("스프라이트 좌표 설정")]
    public int[] xCoords = { 5, 277, 548, 821, 1094, 1365, 1638, 1909, 2180, 2452, 2725 };  // X축 시작 좌표 배열
    public int[] yCoords = { 3, 275, 547, 819, 1091, 1363 };  // Y축 시작 좌표 배열

    [Header("이름 설정 (y행 x열 순서)")]
    public string[,] spriteNames = new string[6, 11]
    {
        { "doublejump1", "doublejump2", "doublejump3", "doublejump4", "doublejump5", "doublejump6", "doublejump0", "jumping0", "jumping1", "slid0", "slid1" },
        { "run0", "run1", "run2", "run3", "fastrun0", "fastrun1", "fastrun2", "fastrun3", "", "", "" },
        { "", "", "", "", "", "", "", "", "", "", "" },
        { "", "", "", "", "", "", "", "", "", "", "" },
        { "", "", "", "", "", "exhaust0", "exhaust1", "exhaust2", "exhaust3", "exhaust4", "" },
        { "", "", "", "", "", "", "", "", "", "", "" },
    };

    [Header("저장 설정")]
    public string baseName = "BraveCookie";             // 파일명 기본 이름
    public string saveFolderPath = "Assets/GeneratedSprites";    // 저장 경로

    [ContextMenu("스프라이트 잘라서 저장하기")]
    public void CutAndSaveSprites()
    {
        if (spriteSheet == null)
        {
            Debug.LogError("SpriteSheet가 할당되지 않았습니다.");
            return;
        }

        if (xCoords.Length != spriteNames.GetLength(1) || yCoords.Length != spriteNames.GetLength(0))
        {
            Debug.LogError("xCoords 또는 yCoords 배열 크기와 spriteNames 배열 크기가 일치하지 않습니다.");
            return;
        }

        if (!AssetDatabase.IsValidFolder(saveFolderPath))
        {
            CreateFolderIfNotExists(saveFolderPath);
        }

        int savedSpriteCount = 0;

        for (int y = 0; y < yCoords.Length; y++)
        {
            for (int x = 0; x < xCoords.Length; x++)
            {
                string spriteName = spriteNames[y, x];
                if (string.IsNullOrWhiteSpace(spriteName))
                {
                    continue;
                }

                float correctedY = spriteSheet.height - yCoords[y] - spriteHeight; // Unity 좌표계 보정
                Rect rect = new Rect(xCoords[x], correctedY, spriteWidth, spriteHeight);

                if (xCoords[x] + spriteWidth > spriteSheet.width || correctedY < 0 || correctedY + spriteHeight > spriteSheet.height)
                {
                    Debug.LogWarning($"잘못된 좌표: x={xCoords[x]}, y={yCoords[y]} (스프라이트 생략)");
                    continue;
                }

                Texture2D newSpriteTexture = new Texture2D(spriteWidth, spriteHeight);
                newSpriteTexture.SetPixels(spriteSheet.GetPixels((int)rect.x, (int)rect.y, spriteWidth, spriteHeight));
                newSpriteTexture.Apply();

                string fileName = $"{baseName}_{spriteName}";
                string fullPath = Path.Combine(saveFolderPath, fileName + ".png");

                SaveTextureAsPNG(newSpriteTexture, fullPath);
                savedSpriteCount++;
            }
        }

        AssetDatabase.Refresh();
        Debug.Log($"총 {savedSpriteCount}개의 스프라이트가 '{saveFolderPath}'에 저장되었습니다.");
    }

    private void SaveTextureAsPNG(Texture2D texture, string path)
    {
        byte[] pngData = texture.EncodeToPNG();
        if (pngData != null)
        {
            File.WriteAllBytes(path, pngData);
            Debug.Log($"저장 완료: {path}");
        }
        else
        {
            Debug.LogError($"PNG 변환 실패: {path}");
        }
    }

    private void CreateFolderIfNotExists(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath)) return;

        string[] pathParts = folderPath.Split('/');
        string currentPath = pathParts[0];

        for (int i = 1; i < pathParts.Length; i++)
        {
            string nextPath = $"{currentPath}/{pathParts[i]}";
            if (!AssetDatabase.IsValidFolder(nextPath))
            {
                AssetDatabase.CreateFolder(currentPath, pathParts[i]);
            }
            currentPath = nextPath;
        }

        AssetDatabase.SaveAssets();
    }
}
