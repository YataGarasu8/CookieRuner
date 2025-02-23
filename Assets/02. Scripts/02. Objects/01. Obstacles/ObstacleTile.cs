using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "ObstacleTile", menuName = "Tile/Obstacle Tile")]
public class ObstacleTile : Tile
{
    public ObstacleData obstacleData;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        // ������ ��忡�� ���� ���� �� obstacleData ���Ҵ� �� ó�� �ߴ�
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return base.StartUp(position, tilemap, go);
        }
#endif

        if (obstacleData == null)
        {
            Debug.LogWarning($"[ObstacleTile] obstacleData�� �Ҵ���� �ʾҽ��ϴ�. ��ġ: {position}");
            return base.StartUp(position, tilemap, go);
        }

        // Tilemap ĳ���� �� ��ȿ�� �˻�
        Tilemap actualTilemap = tilemap.GetComponent<Tilemap>();
        if (actualTilemap == null)
        {
            Debug.LogError("[ObstacleTile] Tilemap ������Ʈ�� ã�� �� �����ϴ�.");
            return base.StartUp(position, tilemap, go);
        }

        // GameObject ����: ���� ������Ʈ�� ������ ���� ����
        GameObject obstacleObject = go != null ? go : new GameObject("ObstacleTileObject");
        obstacleObject.transform.position = actualTilemap.CellToWorld(position) + (Vector3)(actualTilemap.cellSize / 2f);

        // Collider ����
        BoxCollider2D collider = obstacleObject.GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = obstacleObject.AddComponent<BoxCollider2D>();
        }
        collider.size = obstacleData.size;
        collider.isTrigger = false;

        // Obstacle ��ũ��Ʈ ���� �� ������ �Ҵ�
        Obstacle obstacle = obstacleObject.GetComponent<Obstacle>();
        if (obstacle == null)
        {
            obstacle = obstacleObject.AddComponent<Obstacle>();
        }
        obstacle.data = obstacleData;

        return true;
    }
}
