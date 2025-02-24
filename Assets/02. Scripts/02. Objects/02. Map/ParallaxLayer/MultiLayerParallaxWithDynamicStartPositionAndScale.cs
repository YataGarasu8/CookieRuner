using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ParallaxLayerConfig
{
    public string layerName;                      // ���̾� �̸�
    public Sprite backgroundSprite;               // ����� ��������Ʈ �̹���
    public Vector2 scale = Vector2.one;           // x, y ������
    public float parallaxMultiplier = 0.5f;       // �з����� ��� (0~1 ��õ)
    public int imageCount = 3;                    // ������ �̹��� ���� (�ּ� 2�� ����)
    public Vector2 startPosition = Vector2.zero;  // ���̾� ���� ��ġ (������)

    [HideInInspector] public Vector2 lastAppliedScale = Vector2.one;    // ������ ����� ������ �� ����
    [HideInInspector] public Vector2 lastAppliedStartPosition = Vector2.zero; // ������ ����� ���� ��ġ �� ����
}

public class MultiLayerParallaxWithDynamicStartPositionAndScale : MonoBehaviour
{
    [Header("�÷��̾� ����")]
    public Transform playerTransform;             // �÷��̾� ��ġ

    [Header("���̾� ����")]
    public ParallaxLayerConfig backgroundLayer = new ParallaxLayerConfig { layerName = "Background", parallaxMultiplier = 0.2f, imageCount = 4, startPosition = new Vector2(0, 0) };
    public ParallaxLayerConfig midgroundLayer = new ParallaxLayerConfig { layerName = "Midground", parallaxMultiplier = 0.5f, imageCount = 5, startPosition = new Vector2(0, -1) };
    public ParallaxLayerConfig foregroundLayer = new ParallaxLayerConfig { layerName = "Foreground", parallaxMultiplier = 0.8f, imageCount = 6, startPosition = new Vector2(0, -2) };

    private class ParallaxLayer
    {
        public ParallaxLayerConfig config;         // ���� ����
        public Transform parentTransform;          // ���̾� �θ�
        public List<Transform> backgrounds = new(); // ��� ������Ʈ ����Ʈ
        public float backgroundWidth;              // ������ ���� ���� ����
    }

    private Vector3 lastPlayerPosition;            // �÷��̾� ������ ��ġ
    private ParallaxLayer[] layers;                // ��� ���̾� ����

    void Start()
    {
        if (!playerTransform)
        {
            Debug.LogError("�÷��̾� ������ �ʿ��մϴ�.");
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
            // �̹����� ���ٸ� ���̾ ��������� ����.
            // Debug.LogError($"���̾� '{config.layerName}'�� ��������Ʈ�� �������� �ʾҽ��ϴ�.");
            return null;
        }

        if (config.imageCount < 2)
        {
            Debug.LogWarning($"���̾� '{config.layerName}'�� �̹��� ������ �ּ� 2�� �̻��̾�� �մϴ�. �⺻�� 3���� �����մϴ�.");
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

        UpdateLayerProperties(layer, true); // �ʱ� ���� �� ��ġ

        return layer;
    }

    /// ���̾� ��ġ, ������ �� ��� ��ġ�� ������Ʈ�մϴ�.
    private void UpdateLayerProperties(ParallaxLayer layer, bool forceUpdate = false)
    {
        var config = layer.config;

        // ������ �Ǵ� ���� ��ġ�� ����Ǿ��� ���� ������Ʈ
        if (forceUpdate ||
            config.scale != config.lastAppliedScale ||
            config.startPosition != config.lastAppliedStartPosition)
        {
            // �θ� ���� ��ġ ����
            layer.parentTransform.localPosition = new Vector3(config.startPosition.x, config.startPosition.y, 0f);

            // ���� ���� ���� (������ ����)
            SpriteRenderer referenceSR = layer.backgrounds[0].GetComponent<SpriteRenderer>();
            layer.backgroundWidth = referenceSR.size.x * config.scale.x;

            // �� ��� ������Ʈ�� ������ �� ��ġ ������Ʈ
            for (int i = 0; i < layer.backgrounds.Count; i++)
            {
                var bg = layer.backgrounds[i];
                bg.localScale = new Vector3(config.scale.x, config.scale.y, 1f);
                bg.localPosition = new Vector3(i * layer.backgroundWidth, 0f, 0f);
            }

            // ���� ���� ����
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

            // �÷��� �� ������ �Ǵ� ���� ��ġ ���� �� ��� �ݿ�
            UpdateLayerProperties(layer);

            // �з����� �̵�
            layer.parentTransform.position += new Vector3(deltaMovement.x * layer.config.parallaxMultiplier, 0f, 0f);

            // �ݺ� ó��
            float repeatDistance = layer.backgroundWidth * layer.config.imageCount;
            float halfRepeatDistance = repeatDistance / 2f;
            float epsilon = 0.01f; // ��� ��鸲 ����

            foreach (var background in layer.backgrounds)
            {
                float distance = playerTransform.position.x - background.position.x;

                // �÷��̾ �ݺ� �Ÿ��� ���� �̻� �־��� ���� �̵�
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
