// ==========================
// SpriteSheetCutter.cs (������Ʈ: Ư�� ��ǥ�� �̸����� �߶󳻱�, �̸�Ƽ�� ����)
// ==========================

using UnityEditor;
using UnityEngine;
using System.IO;

public class SpriteSheetCutter : MonoBehaviour
{
    [Header("��������Ʈ ��Ʈ ����")]
    public Texture2D spriteSheet; // �ڸ� ��������Ʈ ��Ʈ �̹���
    public int spriteWidth = 264;  // �� ��������Ʈ�� ���� ũ��
    public int spriteHeight = 264; // �� ��������Ʈ�� ���� ũ��

    [Header("��������Ʈ ��ǥ ����")]
    public int[] xCoords = { 5, 277, 548, 821, 1094, 1365, 1638, 1909, 2180, 2452, 2725 };  // X�� ���� ��ǥ �迭
    public int[] yCoords = { 3, 275, 547, 819, 1091, 1363 };  // Y�� ���� ��ǥ �迭

    [Header("�̸� ���� (y�� x�� ����)")]
    public string[,] spriteNames = new string[6, 11]
    {
        { "doublejump1", "doublejump2", "doublejump3", "doublejump4", "doublejump5", "doublejump6", "doublejump0", "jumping0", "jumping1", "slid0", "slid1" },
        { "run0", "run1", "run2", "run3", "fastrun0", "fastrun1", "fastrun2", "fastrun3", "", "", "" },
        { "", "", "", "", "", "", "", "", "", "", "" },
        { "", "", "", "", "", "", "", "", "", "", "" },
        { "", "", "", "", "", "exhaust0", "exhaust1", "exhaust2", "exhaust3", "exhaust4", "" },
        { "", "", "", "", "", "", "", "", "", "", "" },
    };

    [Header("���� ����")]
    public string baseName = "BraveCookie";             // ���ϸ� �⺻ �̸�
    public string saveFolderPath = "Assets/GeneratedSprites";    // ���� ���

    [ContextMenu("��������Ʈ �߶� �����ϱ�")]
    public void CutAndSaveSprites()
    {
        if (spriteSheet == null)
        {
            Debug.LogError("SpriteSheet�� �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }

        if (xCoords.Length != spriteNames.GetLength(1) || yCoords.Length != spriteNames.GetLength(0))
        {
            Debug.LogError("xCoords �Ǵ� yCoords �迭 ũ��� spriteNames �迭 ũ�Ⱑ ��ġ���� �ʽ��ϴ�.");
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

                float correctedY = spriteSheet.height - yCoords[y] - spriteHeight; // Unity ��ǥ�� ����
                Rect rect = new Rect(xCoords[x], correctedY, spriteWidth, spriteHeight);

                if (xCoords[x] + spriteWidth > spriteSheet.width || correctedY < 0 || correctedY + spriteHeight > spriteSheet.height)
                {
                    Debug.LogWarning($"�߸��� ��ǥ: x={xCoords[x]}, y={yCoords[y]} (��������Ʈ ����)");
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
        Debug.Log($"�� {savedSpriteCount}���� ��������Ʈ�� '{saveFolderPath}'�� ����Ǿ����ϴ�.");
    }

    private void SaveTextureAsPNG(Texture2D texture, string path)
    {
        byte[] pngData = texture.EncodeToPNG();
        if (pngData != null)
        {
            File.WriteAllBytes(path, pngData);
            Debug.Log($"���� �Ϸ�: {path}");
        }
        else
        {
            Debug.LogError($"PNG ��ȯ ����: {path}");
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
