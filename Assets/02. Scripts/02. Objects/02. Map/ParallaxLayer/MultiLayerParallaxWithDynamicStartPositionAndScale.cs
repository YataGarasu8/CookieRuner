using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ParallaxLayerConfig
{
    public string layerName;                      // 레이어 이름
    public Sprite backgroundSprite;               // 사용할 스프라이트 이미지
    public Vector2 scale = Vector2.one;           // x, y 스케일
    public float parallaxMultiplier = 0.5f;       // 패럴랙스 계수 (0~1 추천)
    public int imageCount = 3;                    // 생성할 이미지 개수 (최소 2개 권장)
    public Vector2 startPosition = Vector2.zero;  // 레이어 시작 위치 (오프셋)

    [HideInInspector] public Vector2 lastAppliedScale = Vector2.one;    // 이전에 적용된 스케일 값 추적
    [HideInInspector] public Vector2 lastAppliedStartPosition = Vector2.zero; // 이전에 적용된 시작 위치 값 추적
}

public class MultiLayerParallaxWithDynamicStartPositionAndScale : MonoBehaviour
{
    [Header("플레이어 참조")]
    public Transform playerTransform;             // 플레이어 위치

    [Header("레이어 설정")]
    public ParallaxLayerConfig backgroundLayer = new ParallaxLayerConfig { layerName = "Background", parallaxMultiplier = 0.2f, imageCount = 4, startPosition = new Vector2(0, 0) };
    public ParallaxLayerConfig midgroundLayer = new ParallaxLayerConfig { layerName = "Midground", parallaxMultiplier = 0.5f, imageCount = 5, startPosition = new Vector2(0, -1) };
    public ParallaxLayerConfig foregroundLayer = new ParallaxLayerConfig { layerName = "Foreground", parallaxMultiplier = 0.8f, imageCount = 6, startPosition = new Vector2(0, -2) };

    private class ParallaxLayer
    {
        public ParallaxLayerConfig config;         // 설정 참조
        public Transform parentTransform;          // 레이어 부모
        public List<Transform> backgrounds = new(); // 배경 오브젝트 리스트
        public float backgroundWidth;              // 스케일 포함 가로 길이
    }

    private Vector3 lastPlayerPosition;            // 플레이어 마지막 위치
    private ParallaxLayer[] layers;                // 모든 레이어 관리

    void Start()
    {
        if (!playerTransform)
        {
            Debug.LogError("플레이어 참조가 필요합니다.");
            return;
        }

        layers = new ParallaxLayer[]
        {
            CreateLayer(backgroundLayer),
            CreateLayer(midgroundLayer),
            CreateLayer(foregroundLayer)
        };

        lastPlayerPosition = playerTransform.position;
    }

    private ParallaxLayer CreateLayer(ParallaxLayerConfig config)
    {
        if (!config.backgroundSprite)
        {
            // 이미지가 없다면 레이어가 만들어지지 않음.
            // Debug.LogError($"레이어 '{config.layerName}'에 스프라이트가 지정되지 않았습니다.");
            return null;
        }

        if (config.imageCount < 2)
        {
            Debug.LogWarning($"레이어 '{config.layerName}'의 이미지 개수는 최소 2개 이상이어야 합니다. 기본값 3으로 설정합니다.");
            config.imageCount = 3;
        }

        GameObject layerParent = new GameObject(config.layerName);
        layerParent.transform.parent = transform;

        ParallaxLayer layer = new ParallaxLayer { config = config, parentTransform = layerParent.transform };

        for (int i = 0; i < config.imageCount; i++)
        {
            GameObject bg = new GameObject($"{config.layerName}_Part{i + 1}");
            bg.transform.parent = layerParent.transform;

            SpriteRenderer sr = bg.AddComponent<SpriteRenderer>();
            sr.sprite = config.backgroundSprite;

            bg.transform.localScale = new Vector3(config.scale.x, config.scale.y, 1f);

            layer.backgrounds.Add(bg.transform);
        }

        UpdateLayerProperties(layer, true); // 초기 설정 및 배치

        return layer;
    }

    /// 레이어 위치, 스케일 및 배경 배치를 업데이트합니다.
    private void UpdateLayerProperties(ParallaxLayer layer, bool forceUpdate = false)
    {
        var config = layer.config;

        // 스케일 또는 시작 위치가 변경되었을 때만 업데이트
        if (forceUpdate ||
            config.scale != config.lastAppliedScale ||
            config.startPosition != config.lastAppliedStartPosition)
        {
            // 부모 시작 위치 적용
            layer.parentTransform.localPosition = new Vector3(config.startPosition.x, config.startPosition.y, 0f);

            // 가로 길이 재계산 (스케일 포함)
            SpriteRenderer referenceSR = layer.backgrounds[0].GetComponent<SpriteRenderer>();
            layer.backgroundWidth = referenceSR.size.x * config.scale.x;

            // 각 배경 오브젝트의 스케일 및 위치 업데이트
            for (int i = 0; i < layer.backgrounds.Count; i++)
            {
                var bg = layer.backgrounds[i];
                bg.localScale = new Vector3(config.scale.x, config.scale.y, 1f);
                bg.localPosition = new Vector3(i * layer.backgroundWidth, 0f, 0f);
            }

            // 변경 사항 저장
            config.lastAppliedScale = config.scale;
            config.lastAppliedStartPosition = config.startPosition;
        }
    }

    void Update()
    {
        Vector3 deltaMovement = playerTransform.position - lastPlayerPosition;

        foreach (var layer in layers)
        {
            if (layer == null) continue;

            // 플레이 중 스케일 또는 시작 위치 변경 시 즉시 반영
            UpdateLayerProperties(layer);

            // 패럴랙스 이동
            layer.parentTransform.position += new Vector3(deltaMovement.x * layer.config.parallaxMultiplier, 0f, 0f);

            // 반복 처리
            float repeatDistance = layer.backgroundWidth * layer.config.imageCount;
            float halfRepeatDistance = repeatDistance / 2f;
            float epsilon = 0.01f; // 경계 흔들림 방지

            foreach (var background in layer.backgrounds)
            {
                float distance = playerTransform.position.x - background.position.x;

                // 플레이어가 반복 거리의 절반 이상 멀어질 때만 이동
                if (Mathf.Abs(distance) > halfRepeatDistance + epsilon)
                {
                    float direction = Mathf.Sign(distance);
                    background.position += new Vector3(repeatDistance * direction, 0f, 0f);
                }
            }
        }

        lastPlayerPosition = playerTransform.position;
    }
}
