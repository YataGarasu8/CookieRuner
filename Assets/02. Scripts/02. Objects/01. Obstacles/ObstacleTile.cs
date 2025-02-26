using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "ObstacleTile", menuName = "Tile/Obstacle Tile")]
public class ObstacleTile : Tile
{
    public ObstacleData obstacleData;

    // Ÿ�ϸʿ� ��ġ�� ������Ʈ ������ ����
    public GameObject obstaclePrefab;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        // ������ ��忡�� ���� ���� �� obstacleData�� obstaclePrefab ���Ҵ� �� ó�� �ߴ�
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return base.StartUp(position, tilemap, go);
        }
#endif
        // obstaclePrefab�� ����, obstacleData�� ���ٸ� ��� �� ����
        if (obstaclePrefab == null && obstacleData == null)
        {
            Debug.LogWarning($"[ObstacleTile] obstacleData�� obstaclePrefab �� �� �Ҵ���� �ʾҽ��ϴ�. ��ġ: {position}");
            return base.StartUp(position, tilemap, go);
        }

        // Tilemap ĳ���� �� ��ȿ�� �˻�
        Tilemap actualTilemap = tilemap.GetComponent<Tilemap>();
        if (actualTilemap == null)
        {
            Debug.LogError("[ObstacleTile] Tilemap ������Ʈ�� ã�� �� �����ϴ�.");
            return base.StartUp(position, tilemap, go);
        }

        GameObject obstacleObject = null;
        // �����ص� �������� ������ Instantiate
        if (obstaclePrefab != null)
        {
            Debug.Log("�������� �Ҵ�Ǿ� �ֽ��ϴ�. �ν��Ͻ�ȭ�� �����մϴ�.");
            obstacleObject = GameObject.Instantiate(obstaclePrefab);
            obstacleObject.name = obstaclePrefab.name;
            Debug.Log("������ �ν��Ͻ�ȭ�� �Ϸ�Ǿ����ϴ�. ������ ������Ʈ �̸�: " + obstacleObject.name);

            // �ν��Ͻ�ȭ�� ������Ʈ Ȥ�� ���� ������Ʈ�� Obstacle ������Ʈ�� �ִ��� �˻�
            Obstacle obstacleComponent = obstacleObject.GetComponentInChildren<Obstacle>();
            if (obstacleComponent == null)
            {
                Debug.Log("����: �ν��Ͻ�ȭ�� �����տ��� Obstacle ������Ʈ�� ã�� �� �����ϴ�.");
            }
        }
        // �������� ������ ���� ��Ĵ�� ���� �����ϰ� obstacleData ���
        else
        {
            obstacleObject = go != null ? go : new GameObject("ObstacleTileObject");

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
        }

        // Ÿ�ϸ� �� ��ġ�� �°� ��ġ ����
        obstacleObject.transform.position = actualTilemap.CellToWorld(position) + (Vector3)(actualTilemap.cellSize / 2f);

        return true;
    }
}
