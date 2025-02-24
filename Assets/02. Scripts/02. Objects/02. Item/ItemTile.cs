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
        // 에디터 모드에서 실행 방지 및 obstacleData 미할당 시 처리 중단
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return base.StartUp(position, tilemap, go);
        }
#endif

        if (itemdata == null)
        {
            Debug.LogWarning($"[ObstacleTile] obstacleData가 할당되지 않았습니다. 위치: {position}");
            return base.StartUp(position, tilemap, go);
        }

        // Tilemap 캐스팅 및 유효성 검사
        Tilemap actualTilemap = tilemap.GetComponent<Tilemap>();
        if (actualTilemap == null)
        {
            Debug.LogError("[ObstacleTile] Tilemap 컴포넌트를 찾을 수 없습니다.");
            return base.StartUp(position, tilemap, go);
        }

        // GameObject 설정: 기존 오브젝트가 없으면 새로 생성
        GameObject itemObject = go != null ? go : new GameObject("itemTileObject");
        itemObject.transform.position = actualTilemap.CellToWorld(position) + (Vector3)(actualTilemap.cellSize / 2f);

        // Collider 설정
        BoxCollider2D collider = itemObject.GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = itemObject.AddComponent<BoxCollider2D>();
        }
        collider.size = itemdata.size;
        collider.isTrigger = false;

        // Obstacle 스크립트 설정 및 데이터 할당
        ItemSC sc = itemObject.GetComponent<ItemSC>();
        if (sc == null)
        {
            sc = itemObject.AddComponent<ItemSC>();
        }
        sc.data = itemdata;

        return true;
    }
}
