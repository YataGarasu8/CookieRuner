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
    public List<GameObject> tilemapPrefabs; // 사용할 타일맵 프리팹 목록 (순환적으로 사용)

    [Tooltip("플레이어 트랜스폼")]
    public Transform player; // 플레이어의 Transform (위치 기반 타일맵 전환에 사용)

    [Header("패럴랙스 및 페이드 설정")]
    [Tooltip("게임에서 사용할 단일 패럴랙스 레이어")]
    public MultipleParallaxLayer parallaxLayer; // 패럴랙스 레이어 참조 (하나만 사용)

    [Tooltip("페이드 효과를 담당하는 매니저")]
    public FadeManager fadeManager; // 화면 전환 시 페이드 인/아웃 효과를 위한 매니저

    private Queue<GameObject> activeTilemaps = new Queue<GameObject>(); // 활성화된 타일맵을 큐로 관리 (최대 2개 유지)
    private int currentTilemapIndex = 0; // 현재 사용할 타일맵 인덱스 (순환 방식)
    private GameObject currentTilemap; // 현재 플레이어가 위치한 타일맵
    private float currentTilemapThresholdX; // 플레이어가 지날 때 새로운 타일맵을 생성하는 임계값 (현재 타일맵의 3분의 2 지점)

    private bool isChangingTilemap = false; // 코루틴 중복 방지

    // 게임 시작 시 초기 타일맵 생성 및 전환 임계값 설정
    void Start()
    {
        // 타일맵 프리팹 리스트 유효성 검사
        if (tilemapPrefabs == null || tilemapPrefabs.Count == 0)
        {
            Debug.LogError("TilemapManager: 타일맵 프리팹 리스트가 비어 있습니다. 인스펙터에서 설정하세요.");
            return;
        }

        // 플레이어 참조 유효성 검사
        if (!player)
        {
            Debug.LogError("TilemapManager: 플레이어가 할당되지 않았습니다.");
            return;
        }

        // 패럴랙스 레이어 참조 유효성 검사
        if (!parallaxLayer)
        {
            Debug.LogError("TilemapManager: 패럴랙스 레이어가 할당되지 않았습니다.");
            return;
        }

        // 초기 타일맵 생성 및 타일맵 전환 임계값 설정
        SpawnInitialTilemap();
        UpdateTilemapThreshold();
    }

    // 매 프레임 플레이어 위치 확인 및 타일맵 전환 조건 검사
    void Update()
    {
        // 플레이어가 현재 타일맵의 3분의 2 지점을 지났을 때 다음 타일맵 생성
        if (!isChangingTilemap && player.position.x >= currentTilemapThresholdX)
        {
            StartCoroutine(ChangeTilemapWithFade());
        }
    }

    // 게임 시작 시 최초 타일맵 생성 및 패럴랙스 설정 적용
    private void SpawnInitialTilemap()
    {
        // 첫 번째 타일맵 프리팹을 (0,0,0) 위치에 생성
        GameObject initialTilemap = Instantiate(GetNextTilemapPrefab(), Vector3.zero, Quaternion.identity);
        initialTilemap.transform.SetParent(transform); // TilemapManager의 자식으로 설정

        activeTilemaps.Enqueue(initialTilemap); // 큐에 추가하여 활성 타일맵으로 등록
        currentTilemap = initialTilemap; // 현재 타일맵 참조 갱신

        // 생성된 타일맵의 패럴랙스 설정 적용
        ApplyParallaxConfigFromTilemap(initialTilemap);

        // TilemapRenderer 비활성화
        DisableTilemapRendererComponent(currentTilemap);
    }

    void DisableTilemapRendererComponent(GameObject currentTilemap)
    {
        // currentTilemap의 자식 중 "Tilemap(Obstacles)" 오브젝트를 찾음
        Transform obstaclesTransform = currentTilemap.transform.Find("Grid/Tilemap(Obstacles)");
        if (obstaclesTransform != null)
        {
            // TilemapRenderer 컴포넌트를 가져와 비활성화
            TilemapRenderer tilemapRenderer = obstaclesTransform.GetComponent<TilemapRenderer>();
            if (tilemapRenderer != null)
            {
                tilemapRenderer.enabled = false;
            }
            else
            {
                Debug.LogWarning("Tilemap(Obstacles)에서 TilemapRenderer 컴포넌트를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("tilemapPrefabs 내에 'Tilemap(Obstacles)' 오브젝트가 존재하지 않습니다.");
        }
    }


    // 타일맵 전환 시 페이드 효과 적용 및 새로운 타일맵 생성 처리
    private IEnumerator ChangeTilemapWithFade()
    {
        isChangingTilemap = true; // 중복 실행 방지

        // FadeInOut 메서드를 한 번만 호출하여 중간 알파 도달 시 타일맵 전환 처리
        yield return StartCoroutine(fadeManager.FadeInOut(() =>
        {
            // 가장 오래된 타일맵 제거 (큐 관리 우선)
            if (activeTilemaps.Count >= 2)
            {
                RemoveOldestTilemap();
            }

            // 새로운 타일맵 생성
            SpawnNextTilemap();

            // 새로운 타일맵 생성 후 임계값 업데이트
            UpdateTilemapThreshold();
        }));

        isChangingTilemap = false; // 코루틴 종료 시 플래그 해제
    }


    // 새로운 타일맵 생성 및 큐 관리, 패럴랙스 설정 적용
    private void SpawnNextTilemap()
    {
        // 현재 타일맵의 가로 길이 계산
        float tilemapLength = GetTilemapWorldLength(currentTilemap);

        // 새 타일맵 생성 위치 계산 (현재 타일맵 오른쪽 끝에 배치)
        Vector3 spawnPosition = currentTilemap.transform.position + new Vector3(tilemapLength, 0f, 0f);

        // 새 타일맵 생성 및 TilemapManager의 자식으로 설정
        GameObject newTilemap = Instantiate(GetNextTilemapPrefab(), spawnPosition, Quaternion.identity);
        newTilemap.transform.SetParent(transform);

        // 활성 타일맵 큐에 추가하고 현재 타일맵 참조 갱신
        activeTilemaps.Enqueue(newTilemap);
        currentTilemap = newTilemap;

        // 새 타일맵의 패럴랙스 설정 적용 (임계값 업데이트 이후에 호출)
        ApplyParallaxConfigFromTilemap(newTilemap);

        // TilemapRenderer 비활성화
        DisableTilemapRendererComponent(currentTilemap);
    }

    // 가장 오래된 타일맵 제거 및 메모리 최적화
    private void RemoveOldestTilemap()
    {
        if (activeTilemaps.Count == 0) return;

        GameObject oldestTilemap = activeTilemaps.Dequeue(); // 큐에서 가장 오래된 타일맵 제거
        if (oldestTilemap != null)
        {
            Destroy(oldestTilemap); // 메모리 절약을 위해 제거
        }
    }

    // 타일맵에서 ParallaxLayerConfigData를 가져와 패럴랙스 레이어에 적용
    private void ApplyParallaxConfigFromTilemap(GameObject tilemapObj)
    {
        // 타일맵에서 TilemapParallaxData 컴포넌트 가져오기
        var tilemapData = tilemapObj.GetComponent<TilemapParallaxData>();

        // 패럴랙스 설정이 존재하면 적용
        if (tilemapData != null && tilemapData.parallaxConfigData != null)
        {
            parallaxLayer.ApplyParallaxConfig(tilemapData.parallaxConfigData);
        }
        else
        {
            Debug.LogWarning($"TilemapManager: {tilemapObj.name}에 패럴랙스 설정이 없습니다.");
        }
    }

    // 타일맵 프리팹 리스트에서 다음 타일맵을 순환적으로 가져오기
    private GameObject GetNextTilemapPrefab()
    {
        // 현재 인덱스의 타일맵 반환
        GameObject prefab = tilemapPrefabs[currentTilemapIndex];

        // 인덱스를 순환적으로 업데이트
        currentTilemapIndex = (currentTilemapIndex + 1) % tilemapPrefabs.Count;

        return prefab;
    }

    // 현재 타일맵의 3분의 2 지점을 계산하여 전환 임계값을 업데이트
    private void UpdateTilemapThreshold()
    {
        float tilemapLength = GetTilemapWorldLength(currentTilemap);

        // 플레이어가 이 값을 넘으면 다음 타일맵 생성
        currentTilemapThresholdX = currentTilemap.transform.position.x + (tilemapLength * 2f / 3f);
    }

    // 타일맵의 가로 길이를 월드 좌표 기준으로 계산
    private float GetTilemapWorldLength(GameObject tilemapObj)
    {
        // 타일맵 컴포넌트 가져오기
        Tilemap tilemap = tilemapObj.GetComponentInChildren<Tilemap>();

        // 타일맵이 없을 경우 기본 길이 반환
        if (tilemap == null)
        {
            // 임시
            Debug.LogWarning("TilemapManager: Tilemap 컴포넌트를 찾을 수 없습니다. 기본 길이 200 적용.");
            return 200;
        }

        tilemap.CompressBounds(); // 불필요한 빈 영역 제거
        Bounds tilemapBounds = tilemap.localBounds; // 로컬 경계 가져오기

        // 로컬 경계를 월드 단위로 변환하여 가로 길이 반환
        Vector3 worldSize = Vector3.Scale(tilemapBounds.size, tilemap.transform.lossyScale);
        return worldSize.x;
    }
}
