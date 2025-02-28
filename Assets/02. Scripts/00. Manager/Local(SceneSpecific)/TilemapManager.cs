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
    public List<GameObject> tilemapPrefabs; // 순환적으로 사용할 타일맵 프리팹 목록

    [Tooltip("플레이어 트랜스폼")]
    public Transform player; // 플레이어의 위치 기반 타일맵 전환에 사용

    [Header("패럴랙스 및 페이드 설정")]
    [Tooltip("게임에서 사용할 단일 패럴랙스 레이어")]
    public MultipleParallaxLayer parallaxLayer; // 단일 패럴랙스 레이어 참조

    // Tilemap 전환 시 페이드 효과를 담당하는 매니저는 Inspector가 아닌 FadeManager.Instance를 사용합니다.
    // [Tooltip("페이드 효과를 담당하는 매니저")]
    // public FadeManager fadeManager;

    private Queue<GameObject> activeTilemaps = new Queue<GameObject>(); // 활성 타일맵 큐 (최대 2개)
    private int currentTilemapIndex = 0; // 순환 인덱스
    private GameObject currentTilemap; // 현재 플레이어가 위치한 타일맵
    private float currentTilemapThresholdX; // 타일맵 전환 임계값 (타일맵의 2/3 지점)

    private bool isChangingTilemap = false; // 코루틴 중복 방지


    void Start()
    {
        // 타일맵 프리팹 리스트 유효성 검사
        if (tilemapPrefabs == null || tilemapPrefabs.Count == 0)
        {
            Debug.LogError("TilemapManager: 타일맵 프리팹 리스트가 비어 있습니다. 인스펙터에서 설정하세요.");
            return;
        }
        // 플레이어 할당 확인
        if (!player)
        {
            Debug.LogError("TilemapManager: 플레이어가 할당되지 않았습니다.");
            return;
        }
        // 패럴랙스 레이어 할당 확인
        if (!parallaxLayer)
        {
            Debug.LogError("TilemapManager: 패럴랙스 레이어가 할당되지 않았습니다.");
            return;
        }

        // 초기 타일맵 생성 후 전환 임계값 업데이트
        SpawnInitialTilemap();
        UpdateTilemapThreshold();
    }

    void Update()
    {
        // 플레이어가 현재 타일맵의 3분의 2 지점을 지나면 타일맵 전환 조건 만족
        if (player == null)
            return;
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
        // TilemapManager 오브젝트의 자식으로 설정
        initialTilemap.transform.SetParent(transform);
        // 활성 타일맵 큐에 추가
        activeTilemaps.Enqueue(initialTilemap);
        // 현재 타일맵 갱신
        currentTilemap = initialTilemap;
        // 타일맵의 패럴랙스 설정 적용
        ApplyParallaxConfigFromTilemap(initialTilemap);
        // 타일맵 내 장애물의 Renderer, Collider 비활성화
        DisableTilemapRendererComponent(currentTilemap);
    }

    // 타일맵 전환 시 페이드 효과 적용 후 새로운 타일맵 생성 처리
    private IEnumerator ChangeTilemapWithFade()
    {
        isChangingTilemap = true; // 중복 실행 방지

        // FadeManager의 싱글톤 인스턴스를 사용하여 페이드 효과 실행
        yield return StartCoroutine(FadeManager.Instance.FadeInOut(() =>
        {
            // 활성 타일맵이 2개 이상이면 가장 오래된 타일맵 제거
            if (activeTilemaps.Count >= 2)
            {
                RemoveOldestTilemap();
            }
            // 새로운 타일맵 생성
            SpawnNextTilemap();
            // 타일맵 전환 임계값 업데이트
            UpdateTilemapThreshold();
        }));

        isChangingTilemap = false; // 코루틴 종료 후 플래그 해제
    }

    // 새로운 타일맵 생성 및 활성 큐에 추가, 패럴랙스 설정 적용
    private void SpawnNextTilemap()
    {
        // 현재 타일맵의 가로 길이 계산
        float tilemapLength = GetTilemapWorldLength(currentTilemap);
        // 새 타일맵 생성 위치를 현재 타일맵 오른쪽 끝 위치로 설정
        Vector3 spawnPosition = currentTilemap.transform.position + new Vector3(tilemapLength, 0f, 0f);
        // 다음 타일맵 프리팹을 해당 위치에 생성
        GameObject newTilemap = Instantiate(GetNextTilemapPrefab(), spawnPosition, Quaternion.identity);
        // TilemapManager 오브젝트의 자식으로 설정
        newTilemap.transform.SetParent(transform);
        // 활성 타일맵 큐에 추가하고 현재 타일맵 갱신
        activeTilemaps.Enqueue(newTilemap);
        currentTilemap = newTilemap;
        Debug.LogWarning(newTilemap.name);
        // 새 타일맵에 패럴랙스 설정 적용
        ApplyParallaxConfigFromTilemap(newTilemap);

        switch (newTilemap.name)
        {
            case "Stage1(Clone)":
                SoundManager.Instance.StopBGM();
                SoundManager.Instance.PlayBGM("GameSceneBGM01");
                break;
            case "Stage2(Clone)":
                SoundManager.Instance.StopBGM();
                SoundManager.Instance.PlayBGM("GameSceneBGM02");
                break;
            case "Stage3(Clone)":
                SoundManager.Instance.StopBGM();
                SoundManager.Instance.PlayBGM("GameSceneBGM03");
                break;
            case "Bonus Stage(Clone)":
                SoundManager.Instance.StopBGM();
                SoundManager.Instance.PlayBGM("BonusStageBGM02");
                break;
            default:
                Debug.Log("타일맵매니저브금디폴트");
                break;
        }


        // 장애물 Renderer, Collider 비활성화 처리
        DisableTilemapRendererComponent(currentTilemap);

    }

    // 가장 오래된 타일맵 제거 및 메모리 최적화
    private void RemoveOldestTilemap()
    {
        if (activeTilemaps.Count == 0)
            return;
        GameObject oldestTilemap = activeTilemaps.Dequeue();
        if (oldestTilemap != null)
        {
            Destroy(oldestTilemap);
        }
    }

    // 타일맵에서 패럴랙스 설정 데이터를 가져와 패럴랙스 레이어에 적용
    private void ApplyParallaxConfigFromTilemap(GameObject tilemapObj)
    {
        var tilemapData = tilemapObj.GetComponent<TilemapParallaxData>();
        if (tilemapData != null && tilemapData.parallaxConfigData != null)
        {
            parallaxLayer.ApplyParallaxConfig(tilemapData.parallaxConfigData);
        }
        else
        {
            Debug.LogWarning($"TilemapManager: {tilemapObj.name}에 패럴랙스 설정이 없습니다.");
        }
    }

    // 타일맵 프리팹 리스트에서 순환적으로 다음 프리팹 반환
    private GameObject GetNextTilemapPrefab()
    {
        GameObject prefab = tilemapPrefabs[currentTilemapIndex];
        currentTilemapIndex = (currentTilemapIndex + 1) % tilemapPrefabs.Count;
        return prefab;
    }

    // 현재 타일맵의 3분의 2 지점을 기준으로 전환 임계값을 업데이트
    // 만약 플레이어의 위치가 이미 계산된 임계값보다 크다면, 플레이어 위치에 tilemap 길이의 1/3을 더한 값으로 보정함
    private void UpdateTilemapThreshold()
    {
        float tilemapLength = GetTilemapWorldLength(currentTilemap);
        float newThreshold = currentTilemap.transform.position.x + (tilemapLength * 2f / 3f);
        if (player.position.x >= newThreshold)
        {
            newThreshold = player.position.x + (tilemapLength / 3f);
        }
        currentTilemapThresholdX = newThreshold;
        Debug.Log($"임계값 업데이트: currentTilemapThresholdX = {currentTilemapThresholdX}");
    }

    // 타일맵의 가로 길이를 월드 좌표 기준으로 계산
    private float GetTilemapWorldLength(GameObject tilemapObj)
    {
        Tilemap tilemap = tilemapObj.GetComponentInChildren<Tilemap>();
        if (tilemap == null)
        {
            Debug.LogWarning("TilemapManager: Tilemap 컴포넌트를 찾을 수 없습니다. 기본 길이 200 적용.");
            return 200;
        }
        tilemap.CompressBounds();
        Bounds tilemapBounds = tilemap.localBounds;
        Vector3 worldSize = Vector3.Scale(tilemapBounds.size, tilemap.transform.lossyScale);
        return worldSize.x;
    }

    // 타일맵 내 장애물 오브젝트의 TilemapRenderer와 TilemapCollider2D 컴포넌트를 비활성화
    void DisableTilemapRendererComponent(GameObject tilemapObj)
    {
        List<Transform> obstacles = new List<Transform>();
        for (int i = 1; i < 13; i++)
        {
            Transform t = tilemapObj.transform.Find($"Grid/Tilemap({i}Obstacles)");
            if (t != null)
                obstacles.Add(t);
        }
        if (obstacles != null)
        {
            for (int i = 0; i < obstacles.Count; i++)
            {
                TilemapRenderer tilemapRenderer = obstacles[i].GetComponent<TilemapRenderer>();
                if (tilemapRenderer != null)
                    tilemapRenderer.enabled = false;
                else
                    Debug.LogWarning("Tilemap(Obstacles)에서 TilemapRenderer 컴포넌트를 찾을 수 없습니다.");
                TilemapCollider2D tilemapCollider2D = obstacles[i].GetComponent<TilemapCollider2D>();
                if (tilemapCollider2D != null)
                    tilemapCollider2D.enabled = false;
                else
                    Debug.LogWarning("Tilemap(Obstacles)에서 TilemapCollider2D 컴포넌트를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("tilemapPrefabs 내에 'Tilemap(Obstacles)' 오브젝트가 존재하지 않습니다.");
        }
    }
}
