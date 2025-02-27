using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 패럴랙스 레이어의 구성 정보를 담는 클래스
// 각 레이어의 이름, 배경 스프라이트, 스케일, 패럴랙스 속도, 이미지 개수, 시작 위치 등을 설정
[System.Serializable]
public class ParallaxLayerConfig
{
    public string layerName;                      // 레이어 이름
    public Sprite backgroundSprite;               // 사용할 스프라이트 이미지
    public Vector2 scale = Vector2.one;           // 레이어 스케일 (x, y 값 조절)
    public float parallaxMultiplier = 0.5f;       // 패럴랙스 계수 (작을수록 이동 속도가 느려짐)
    public int imageCount = 3;                    // 반복될 배경 이미지 개수 (최소 2개 권장)
    public Vector2 startPosition = Vector2.zero;  // 레이어 시작 위치 (배치 시 오프셋 적용)

    [HideInInspector] public Vector2 lastAppliedScale = Vector2.one;         // 이전에 적용된 스케일 값 (중복 업데이트 방지용)
    [HideInInspector] public Vector2 lastAppliedStartPosition = Vector2.zero; // 이전에 적용된 시작 위치 값 (중복 업데이트 방지용)

    // MODIFIED: 각 레이어의 Order in Layer를 설정할 수 있도록 추가 (인스펙터에서 설정 가능)
    public int sortingOrder = 0;
}

// 패럴랙스 레이어들을 관리하며 플레이어 이동에 따라 배경을 반복적으로 스크롤하는 클래스
// 외부에서 ScriptableObject(ParallaxLayerConfigData)를 받아 레이어 설정을 적용
public class MultipleParallaxLayer : MonoBehaviour
{
    [Header("플레이어 참조")]
    public Transform playerTransform; // 플레이어의 위치를 참조하여 패럴랙스 효과에 적용

    // 내부적으로 사용할 레이어 정보를 담는 클래스
    private class ParallaxLayer
    {
        public ParallaxLayerConfig config;          // 해당 레이어의 설정 데이터
        public Transform parentTransform;           // 레이어의 부모 오브젝트 (배경 이미지들을 담는 컨테이너)
        public List<Transform> backgrounds = new(); // 배경 이미지 오브젝트 리스트
        public float backgroundWidth;               // 각 배경 이미지의 가로 길이 (스케일 포함)
    }

    private Vector3 lastPlayerPosition;             // 플레이어의 마지막 위치 (이동량 계산에 사용)
    private ParallaxLayer[] layers;                 // 모든 패럴랙스 레이어를 관리하는 배열

    // 게임 시작 시 초기화
    void Start()
    {
        // 플레이어 참조가 할당되지 않은 경우 경고 출력
        if (!playerTransform)
        {
            Debug.LogError("MultipleParallaxLayer: 플레이어 참조가 필요합니다.");
            return;
        }

        // 플레이어의 현재 위치를 기록하여 이후 이동량 계산 시 사용
        lastPlayerPosition = playerTransform.position;
    }

    // 외부에서 패럴랙스 설정 데이터를 적용하는 메서드
    // 기존 레이어들을 제거한 뒤 새 설정에 따라 레이어를 재생성
    public void ApplyParallaxConfig(ParallaxLayerConfigData configData)
    {
        if (configData == null)
        {
            Debug.LogError("MultipleParallaxLayer: 패럴랙스 설정 데이터가 null입니다.");
            return;
        }

        // MODIFIED: 기존 레이어를 제거하여 새 맵의 configData가 적용되도록 함
        ClearExistingLayers();

        // MODIFIED: 새로운 configData를 기반으로 레이어 재생성
        // 이제 각 레이어의 sortingOrder는 configData에 설정된 값을 사용합니다.
        layers = new ParallaxLayer[]
        {
            CreateLayer(configData.backgroundLayer),
            CreateLayer(configData.midgroundLayer),
            CreateLayer(configData.foregroundLayer)
        };

        // 플레이어 현재 위치 기록
        lastPlayerPosition = playerTransform.position;
    }

    // 기존 레이어의 설정만 업데이트하는 메서드
    private void UpdateLayerSettings(ParallaxLayer layer, ParallaxLayerConfig config)
    {
        if (layer == null)
            return;

        layer.config = config; // 새 설정 적용
        UpdateLayerProperties(layer, true); // 레이어 속성 갱신
    }

    // 기존 레이어를 제거하는 메서드
    // 레이어의 부모 오브젝트를 제거하여 해당 레이어에 포함된 모든 배경 오브젝트를 삭제
    private void ClearExistingLayers()
    {
        if (layers == null) return;

        foreach (var layer in layers)
        {
            if (layer?.parentTransform != null)
            {
                Destroy(layer.parentTransform.gameObject); // 부모 오브젝트 제거 시 하위 배경 오브젝트도 함께 제거됨
            }
        }
    }

    // 주어진 설정을 기반으로 새로운 패럴랙스 레이어를 생성하는 메서드
    // MODIFIED: sortingOrder 매개변수를 제거하고, config의 sortingOrder 값을 사용하도록 변경
    private ParallaxLayer CreateLayer(ParallaxLayerConfig config)
    {
        // 스프라이트가 지정되지 않은 경우 레이어 생성하지 않고 null 반환
        // (기존 주석 그대로 유지)
        if (!config.backgroundSprite)
        {
            Debug.LogWarning($"MultipleParallaxLayer: 레이어 '{config.layerName}'에 스프라이트가 지정되지 않았습니다. 의도했다면 정상입니다.");
            // 스프라이트가 없더라도 레이어 생성 (null 반환하지 않음)
        }

        // 이미지 개수가 최소값(2)보다 작은 경우 경고 출력 후 기본값 적용
        if (config.imageCount < 2)
        {
            Debug.LogWarning($"MultipleParallaxLayer: 레이어 '{config.layerName}'의 이미지 개수는 최소 2개 이상이어야 합니다. 기본값 3으로 설정합니다.");
            config.imageCount = 3;
        }

        // 레이어 부모 오브젝트 생성 (배경 이미지들을 담을 컨테이너 역할)
        GameObject layerParent = new GameObject(config.layerName);
        layerParent.transform.parent = transform; // 해당 스크립트 오브젝트의 자식으로 설정

        // 레이어 객체 생성 및 설정 적용
        ParallaxLayer layer = new ParallaxLayer
        {
            config = config,
            parentTransform = layerParent.transform
        };

        // 설정된 이미지 개수만큼 배경 오브젝트 생성 및 배치
        for (int i = 0; i < config.imageCount; i++)
        {
            GameObject bg = new GameObject($"{config.layerName}_Part{i + 1}");
            bg.transform.parent = layerParent.transform; // 레이어 부모의 자식으로 설정

            SpriteRenderer sr = bg.AddComponent<SpriteRenderer>();
            sr.sprite = config.backgroundSprite; // 설정된 스프라이트 적용
            // MODIFIED: SpriteRenderer의 sortingOrder를 config에서 설정한 값으로 지정
            sr.sortingOrder = config.sortingOrder;

            bg.transform.localScale = new Vector3(config.scale.x, config.scale.y, 1f); // 스케일 적용
            layer.backgrounds.Add(bg.transform); // 생성된 배경을 레이어 리스트에 추가
        }

        // 배경 이미지 초기 위치 및 크기 적용
        UpdateLayerProperties(layer, true);

        return layer;
    }

    // 주어진 레이어의 위치, 스케일, 배경 배치를 업데이트하는 메서드
    // forceUpdate가 true일 경우 변경 여부와 상관없이 강제로 업데이트를 수행
    private void UpdateLayerProperties(ParallaxLayer layer, bool forceUpdate = false)
    {
        var config = layer.config;

        // config 데이터의 sortingOrder 값이 변경되었을 수 있으므로 각 배경 오브젝트의 sortingOrder를 갱신
        foreach (var bg in layer.backgrounds)
        {
            SpriteRenderer sr = bg.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sortingOrder != config.sortingOrder)
            {
                sr.sortingOrder = config.sortingOrder;
            }
        }

        // 스케일이나 시작 위치가 이전 값과 다르거나 강제 업데이트 요청 시 실행
        if (forceUpdate || config.scale != config.lastAppliedScale || config.startPosition != config.lastAppliedStartPosition)
        {
            // 레이어 부모의 위치를 설정 (시작 위치 오프셋 적용)
            layer.parentTransform.localPosition = new Vector3(config.startPosition.x, config.startPosition.y, 0f);

            // 첫 번째 배경 오브젝트를 기준으로 가로 길이 계산
            SpriteRenderer referenceSR = layer.backgrounds[0].GetComponent<SpriteRenderer>();
            layer.backgroundWidth = referenceSR.size.x * config.scale.x;

            // 모든 배경 오브젝트의 위치와 스케일 재설정
            for (int i = 0; i < layer.backgrounds.Count; i++)
            {
                var bg = layer.backgrounds[i];
                bg.localScale = new Vector3(config.scale.x, config.scale.y, 1f); // 스케일 적용
                bg.localPosition = new Vector3(i * layer.backgroundWidth, 0f, 0f); // 연속적으로 이어지도록 배치
            }

            // 적용된 스케일과 시작 위치 저장 (다음 업데이트 시 비교용)
            config.lastAppliedScale = config.scale;
            config.lastAppliedStartPosition = config.startPosition;
        }
    }


    // 매 프레임 호출되어 패럴랙스 효과를 적용하는 메서드
    void Update()
    {
        if (layers == null || playerTransform == null) return;

        // 플레이어 위치 변화량 계산
        Vector3 deltaMovement = playerTransform.position - lastPlayerPosition;

        foreach (var layer in layers)
        {
            if (layer == null) continue;

            // 스케일 및 위치 업데이트 (설정 변경 시 즉시 반영)
            UpdateLayerProperties(layer);

            // 레이어 이동 적용 (패럴랙스 계수에 따라 속도 다름)
            layer.parentTransform.position += new Vector3(deltaMovement.x * layer.config.parallaxMultiplier, 0f, 0f);

            // 배경 반복 처리
            float repeatDistance = layer.backgroundWidth * layer.config.imageCount; // 전체 배경 길이
            float halfRepeatDistance = repeatDistance / 2f; // 반복 기준 거리

            foreach (var background in layer.backgrounds)
            {
                float distance = playerTransform.position.x - background.position.x; // 플레이어와 배경 사이 거리 계산

                // 플레이어가 반복 거리의 절반 이상 멀어질 경우 배경을 반복 위치로 이동
                if (Mathf.Abs(distance) > halfRepeatDistance)
                {
                    background.position += new Vector3(repeatDistance * Mathf.Sign(distance), 0f, 0f); // 배경 반복 위치로 이동
                }
            }
        }

        // 플레이어 위치 기록 (다음 프레임에서 이동량 계산 시 사용)
        lastPlayerPosition = playerTransform.position;
    }
}
