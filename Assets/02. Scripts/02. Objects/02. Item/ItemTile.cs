using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "ItemTile", menuName = "Tile/Item Tile")]
public class ItemTile : Tile
{
    public Item itemdata;
    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        // ������ ��忡�� ���� ���� �� obstacleData ���Ҵ� �� ó�� �ߴ�
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return base.StartUp(position, tilemap, go);
        }
#endif

        if (itemdata == null)
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
        GameObject itemObject = go != null ? go : new GameObject("itemTileObject");
        itemObject.transform.position = actualTilemap.CellToWorld(position) + (Vector3)(actualTilemap.cellSize / 2f);

        // Collider ����
        BoxCollider2D collider = itemObject.GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = itemObject.AddComponent<BoxCollider2D>();
        }
        collider.size = itemdata.size;
        collider.isTrigger = false;

        // Obstacle ��ũ��Ʈ ���� �� ������ �Ҵ�
        ItemSC sc = itemObject.GetComponent<ItemSC>();
        if (sc == null)
        {
            sc = itemObject.AddComponent<ItemSC>();
        }
        sc.data = itemdata;

        return true;
    }
}
