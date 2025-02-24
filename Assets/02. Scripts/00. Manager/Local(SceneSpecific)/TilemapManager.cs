using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// 타일맵을 관리하며 플레이어 이동에 따라 타일맵 생성 및 제거를 담당하는 클래스
// - 인스펙터 창에서 여러 타일맵 프리팹을 받아 순서대로 사용
// - 활성화된 타일맵 개수는 2개로 제한
// - 현재 플레이어가 위치한 타일맵의 길이를 가져와 해당 위치에 이어서 다음 타일맵 생성
// - 플레이어가 타일맵의 3분의 2 지점을 지나면 새로운 타일맵 생성 및 이전 타일맵 제거
public class TilemapManager : MonoBehaviour
{
    [Header("타일맵 관리 설정")]
    [Tooltip("사용할 타일맵 프리팹 리스트 (생성 순서대로 추가)")]
    public List<GameObject> tilemapPrefabs;       // 사용할 타일맵 프리팹 리스트

    [Tooltip("플레이어 트랜스폼")]
    public Transform player;                      // 플레이어 참조

    private Queue<GameObject> activeTilemaps = new Queue<GameObject>(); // 활성 타일맵 큐 (최대 2개 유지)
    private int currentTilemapIndex = 0;                                // 현재 사용할 타일맵 인덱스
    private GameObject currentTilemap;                                  // 현재 플레이어가 위치한 타일맵
    private float currentTilemapThresholdX;                             // 현재 타일맵의 3분의 2 지점 X 좌표

    void Start()
    {
        if (tilemapPrefabs == null || tilemapPrefabs.Count == 0)
        {
            Debug.LogError("TilemapManager: 타일맵 프리팹 리스트가 비어 있습니다. 인스펙터에서 설정하세요.");
            return;
        }

        if (!player)
        {
            Debug.LogError("TilemapManager: 플레이어가 할당되지 않았습니다.");
            return;
        }

        // 초기 타일맵 생성 및 임계 지점 설정
        SpawnInitialTilemap();
        UpdateTilemapThreshold();
    }

    void Update()
    {
        if (player.position.x >= currentTilemapThresholdX)
        {
            SpawnTilemap();           // 다음 타일맵 생성
            UpdateTilemapThreshold(); // 새로운 타일맵 임계값 갱신

            // 최대 활성 타일맵 수(2개) 초과 시 이전 타일맵 제거
            if (activeTilemaps.Count > 2)
            {
                RemoveOldestTilemap();
            }
        }
    }

    // 초기 타일맵 생성 및 큐에 추가
    private void SpawnInitialTilemap()
    {
        GameObject initialTilemap = Instantiate(GetNextTilemapPrefab(), Vector3.zero, Quaternion.identity);
        initialTilemap.transform.SetParent(transform); // TilemapManager 하위에 배치
        activeTilemaps.Enqueue(initialTilemap);
        currentTilemap = initialTilemap;

        // parallaxLayer 찾기 및 플레이어 transform 할당
        var parallaxLayer = currentTilemap.GetComponentInChildren<MultiLayerParallaxWithDynamicStartPositionAndScale>();
        if (parallaxLayer != null)
        {
            parallaxLayer.playerTransform = player;
        }
        else
        {
            Debug.LogWarning("parallaxLayer를 찾을 수 없습니다.");
        }
    }

    // 새로운 타일맵 생성 및 큐에 추가
    private void SpawnTilemap()
    {
        float tilemapLength = GetTilemapWorldLength(currentTilemap);
        Vector3 spawnPosition = currentTilemap.transform.position + new Vector3(tilemapLength, 0f, 0f);

        GameObject newTilemap = Instantiate(GetNextTilemapPrefab(), spawnPosition, Quaternion.identity);
        newTilemap.transform.SetParent(transform); // TilemapManager 하위에 배치

        // parallaxLayer 찾기 및 플레이어 transform 할당
        var parallaxLayer = newTilemap.GetComponentInChildren<MultiLayerParallaxWithDynamicStartPositionAndScale>();
        if (parallaxLayer != null)
        {
            parallaxLayer.playerTransform = player;
        }
        else
        {
            Debug.LogWarning("parallaxLayer를 찾을 수 없습니다.");
        }

        activeTilemaps.Enqueue(newTilemap); // 큐에 추가
        currentTilemap = newTilemap;        // 현재 타일맵 업데이트
    }

    // 타일맵 프리팹 리스트에서 다음 타일맵 가져오기 (순환)
    private GameObject GetNextTilemapPrefab()
    {
        GameObject prefab = tilemapPrefabs[currentTilemapIndex];
        currentTilemapIndex = (currentTilemapIndex + 1) % tilemapPrefabs.Count; // 인덱스 순환
        return prefab;
    }

    // 현재 타일맵의 3분의 2 지점 계산 및 업데이트
    private void UpdateTilemapThreshold()
    {
        float tilemapLength = GetTilemapWorldLength(currentTilemap);
        currentTilemapThresholdX = currentTilemap.transform.position.x + (tilemapLength * 2f / 3f);
    }

    // 가장 오래된 타일맵 제거
    private void RemoveOldestTilemap()
    {
        if (activeTilemaps.Count == 0) return;

        GameObject oldestTilemap = activeTilemaps.Dequeue();
        if (oldestTilemap != null)
        {
            Destroy(oldestTilemap); // 메모리 절약을 위해 제거
        }
    }

    // 타일맵의 가로 길이를 월드 좌표 기준으로 계산
    private float GetTilemapWorldLength(GameObject tilemapObj)
    {
        Tilemap tilemap = tilemapObj.GetComponentInChildren<Tilemap>();

        if (tilemap == null)
        {
            Debug.LogWarning("TilemapManager: Tilemap 컴포넌트를 찾을 수 없습니다. 기본 길이 10 적용.");
            return 10f; // 기본 길이 적용
        }

        tilemap.CompressBounds(); // 타일맵 경계 최적화
        Bounds tilemapBounds = tilemap.localBounds; // 로컬 경계 가져오기

        // 로컬 경계를 월드 단위로 변환
        Vector3 worldSize = Vector3.Scale(tilemapBounds.size, tilemap.transform.lossyScale);
        return worldSize.x;
    }
}
