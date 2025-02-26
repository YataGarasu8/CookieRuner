using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "ObstacleTile", menuName = "Tile/Obstacle Tile")]
public class ObstacleTile : Tile
{
    public ObstacleData obstacleData;

    // 타일맵에 배치할 오브젝트 프리팹 참조
    public GameObject obstaclePrefab;

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        // 에디터 모드에서 실행 방지 및 obstacleData와 obstaclePrefab 미할당 시 처리 중단
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            return base.StartUp(position, tilemap, go);
        }
#endif
        // obstaclePrefab이 없고, obstacleData도 없다면 경고 후 종료
        if (obstaclePrefab == null && obstacleData == null)
        {
            Debug.LogWarning($"[ObstacleTile] obstacleData와 obstaclePrefab 둘 다 할당되지 않았습니다. 위치: {position}");
            return base.StartUp(position, tilemap, go);
        }

        // Tilemap 캐스팅 및 유효성 검사
        Tilemap actualTilemap = tilemap.GetComponent<Tilemap>();
        if (actualTilemap == null)
        {
            Debug.LogError("[ObstacleTile] Tilemap 컴포넌트를 찾을 수 없습니다.");
            return base.StartUp(position, tilemap, go);
        }

        GameObject obstacleObject = null;
        // 설정해둔 프리팹이 있으면 Instantiate
        if (obstaclePrefab != null)
        {
            Debug.Log("프리팹이 할당되어 있습니다. 인스턴스화를 시작합니다.");
            obstacleObject = GameObject.Instantiate(obstaclePrefab);
            obstacleObject.name = obstaclePrefab.name;
            Debug.Log("프리팹 인스턴스화가 완료되었습니다. 생성된 오브젝트 이름: " + obstacleObject.name);

            // 인스턴스화된 오브젝트 혹은 하위 오브젝트에 Obstacle 컴포넌트가 있는지 검사
            Obstacle obstacleComponent = obstacleObject.GetComponentInChildren<Obstacle>();
            if (obstacleComponent == null)
            {
                Debug.Log("오류: 인스턴스화된 프리팹에서 Obstacle 컴포넌트를 찾을 수 없습니다.");
            }
        }
        // 프리팹이 없으면 기존 방식대로 새로 생성하고 obstacleData 사용
        else
        {
            obstacleObject = go != null ? go : new GameObject("ObstacleTileObject");

            // Collider 설정
            BoxCollider2D collider = obstacleObject.GetComponent<BoxCollider2D>();
            if (collider == null)
            {
                collider = obstacleObject.AddComponent<BoxCollider2D>();
            }
            collider.size = obstacleData.size;
            collider.isTrigger = false;

            // Obstacle 스크립트 설정 및 데이터 할당
            Obstacle obstacle = obstacleObject.GetComponent<Obstacle>();
            if (obstacle == null)
            {
                obstacle = obstacleObject.AddComponent<Obstacle>();
            }
            obstacle.data = obstacleData;
        }

        // 타일맵 셀 위치에 맞게 위치 설정
        obstacleObject.transform.position = actualTilemap.CellToWorld(position) + (Vector3)(actualTilemap.cellSize / 2f);

        return true;
    }
}
