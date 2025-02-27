using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �з����� ���̾��� ���� ������ ��� Ŭ����
// �� ���̾��� �̸�, ��� ��������Ʈ, ������, �з����� �ӵ�, �̹��� ����, ���� ��ġ ���� ����
[System.Serializable]
public class ParallaxLayerConfig
{
    public string layerName;                      // ���̾� �̸�
    public Sprite backgroundSprite;               // ����� ��������Ʈ �̹���
    public Vector2 scale = Vector2.one;           // ���̾� ������ (x, y �� ����)
    public float parallaxMultiplier = 0.5f;       // �з����� ��� (�������� �̵� �ӵ��� ������)
    public int imageCount = 3;                    // �ݺ��� ��� �̹��� ���� (�ּ� 2�� ����)
    public Vector2 startPosition = Vector2.zero;  // ���̾� ���� ��ġ (��ġ �� ������ ����)

    [HideInInspector] public Vector2 lastAppliedScale = Vector2.one;         // ������ ����� ������ �� (�ߺ� ������Ʈ ������)
    [HideInInspector] public Vector2 lastAppliedStartPosition = Vector2.zero; // ������ ����� ���� ��ġ �� (�ߺ� ������Ʈ ������)

    // MODIFIED: �� ���̾��� Order in Layer�� ������ �� �ֵ��� �߰� (�ν����Ϳ��� ���� ����)
    public int sortingOrder = 0;
}

// �з����� ���̾���� �����ϸ� �÷��̾� �̵��� ���� ����� �ݺ������� ��ũ���ϴ� Ŭ����
// �ܺο��� ScriptableObject(ParallaxLayerConfigData)�� �޾� ���̾� ������ ����
public class MultipleParallaxLayer : MonoBehaviour
{
    [Header("�÷��̾� ����")]
    public Transform playerTransform; // �÷��̾��� ��ġ�� �����Ͽ� �з����� ȿ���� ����

    // ���������� ����� ���̾� ������ ��� Ŭ����
    private class ParallaxLayer
    {
        public ParallaxLayerConfig config;          // �ش� ���̾��� ���� ������
        public Transform parentTransform;           // ���̾��� �θ� ������Ʈ (��� �̹������� ��� �����̳�)
        public List<Transform> backgrounds = new(); // ��� �̹��� ������Ʈ ����Ʈ
        public float backgroundWidth;               // �� ��� �̹����� ���� ���� (������ ����)
    }

    private Vector3 lastPlayerPosition;             // �÷��̾��� ������ ��ġ (�̵��� ��꿡 ���)
    private ParallaxLayer[] layers;                 // ��� �з����� ���̾ �����ϴ� �迭

    // ���� ���� �� �ʱ�ȭ
    void Start()
    {
        // �÷��̾� ������ �Ҵ���� ���� ��� ��� ���
        if (!playerTransform)
        {
            Debug.LogError("MultipleParallaxLayer: �÷��̾� ������ �ʿ��մϴ�.");
            return;
        }

        // �÷��̾��� ���� ��ġ�� ����Ͽ� ���� �̵��� ��� �� ���
        lastPlayerPosition = playerTransform.position;
    }

    // �ܺο��� �з����� ���� �����͸� �����ϴ� �޼���
    // ���� ���̾���� ������ �� �� ������ ���� ���̾ �����
    public void ApplyParallaxConfig(ParallaxLayerConfigData configData)
    {
        if (configData == null)
        {
            Debug.LogError("MultipleParallaxLayer: �з����� ���� �����Ͱ� null�Դϴ�.");
            return;
        }

        // MODIFIED: ���� ���̾ �����Ͽ� �� ���� configData�� ����ǵ��� ��
        ClearExistingLayers();

        // MODIFIED: ���ο� configData�� ������� ���̾� �����
        // ���� �� ���̾��� sortingOrder�� configData�� ������ ���� ����մϴ�.
        layers = new ParallaxLayer[]
        {
            CreateLayer(configData.backgroundLayer),
            CreateLayer(configData.midgroundLayer),
            CreateLayer(configData.foregroundLayer)
        };

        // �÷��̾� ���� ��ġ ���
        lastPlayerPosition = playerTransform.position;
    }

    // ���� ���̾��� ������ ������Ʈ�ϴ� �޼���
    private void UpdateLayerSettings(ParallaxLayer layer, ParallaxLayerConfig config)
    {
        if (layer == null)
            return;

        layer.config = config; // �� ���� ����
        UpdateLayerProperties(layer, true); // ���̾� �Ӽ� ����
    }

    // ���� ���̾ �����ϴ� �޼���
    // ���̾��� �θ� ������Ʈ�� �����Ͽ� �ش� ���̾ ���Ե� ��� ��� ������Ʈ�� ����
    private void ClearExistingLayers()
    {
        if (layers == null) return;

        foreach (var layer in layers)
        {
            if (layer?.parentTransform != null)
            {
                Destroy(layer.parentTransform.gameObject); // �θ� ������Ʈ ���� �� ���� ��� ������Ʈ�� �Բ� ���ŵ�
            }
        }
    }

    // �־��� ������ ������� ���ο� �з����� ���̾ �����ϴ� �޼���
    // MODIFIED: sortingOrder �Ű������� �����ϰ�, config�� sortingOrder ���� ����ϵ��� ����
    private ParallaxLayer CreateLayer(ParallaxLayerConfig config)
    {
        // ��������Ʈ�� �������� ���� ��� ���̾� �������� �ʰ� null ��ȯ
        // (���� �ּ� �״�� ����)
        if (!config.backgroundSprite)
        {
            Debug.LogWarning($"MultipleParallaxLayer: ���̾� '{config.layerName}'�� ��������Ʈ�� �������� �ʾҽ��ϴ�. �ǵ��ߴٸ� �����Դϴ�.");
            // ��������Ʈ�� ������ ���̾� ���� (null ��ȯ���� ����)
        }

        // �̹��� ������ �ּҰ�(2)���� ���� ��� ��� ��� �� �⺻�� ����
        if (config.imageCount < 2)
        {
            Debug.LogWarning($"MultipleParallaxLayer: ���̾� '{config.layerName}'�� �̹��� ������ �ּ� 2�� �̻��̾�� �մϴ�. �⺻�� 3���� �����մϴ�.");
            config.imageCount = 3;
        }

        // ���̾� �θ� ������Ʈ ���� (��� �̹������� ���� �����̳� ����)
        GameObject layerParent = new GameObject(config.layerName);
        layerParent.transform.parent = transform; // �ش� ��ũ��Ʈ ������Ʈ�� �ڽ����� ����

        // ���̾� ��ü ���� �� ���� ����
        ParallaxLayer layer = new ParallaxLayer
        {
            config = config,
            parentTransform = layerParent.transform
        };

        // ������ �̹��� ������ŭ ��� ������Ʈ ���� �� ��ġ
        for (int i = 0; i < config.imageCount; i++)
        {
            GameObject bg = new GameObject($"{config.layerName}_Part{i + 1}");
            bg.transform.parent = layerParent.transform; // ���̾� �θ��� �ڽ����� ����

            SpriteRenderer sr = bg.AddComponent<SpriteRenderer>();
            sr.sprite = config.backgroundSprite; // ������ ��������Ʈ ����
            // MODIFIED: SpriteRenderer�� sortingOrder�� config���� ������ ������ ����
            sr.sortingOrder = config.sortingOrder;

            bg.transform.localScale = new Vector3(config.scale.x, config.scale.y, 1f); // ������ ����
            layer.backgrounds.Add(bg.transform); // ������ ����� ���̾� ����Ʈ�� �߰�
        }

        // ��� �̹��� �ʱ� ��ġ �� ũ�� ����
        UpdateLayerProperties(layer, true);

        return layer;
    }

    // �־��� ���̾��� ��ġ, ������, ��� ��ġ�� ������Ʈ�ϴ� �޼���
    // forceUpdate�� true�� ��� ���� ���ο� ������� ������ ������Ʈ�� ����
    private void UpdateLayerProperties(ParallaxLayer layer, bool forceUpdate = false)
    {
        var config = layer.config;

        // config �������� sortingOrder ���� ����Ǿ��� �� �����Ƿ� �� ��� ������Ʈ�� sortingOrder�� ����
        foreach (var bg in layer.backgrounds)
        {
            SpriteRenderer sr = bg.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sortingOrder != config.sortingOrder)
            {
                sr.sortingOrder = config.sortingOrder;
            }
        }

        // �������̳� ���� ��ġ�� ���� ���� �ٸ��ų� ���� ������Ʈ ��û �� ����
        if (forceUpdate || config.scale != config.lastAppliedScale || config.startPosition != config.lastAppliedStartPosition)
        {
            // ���̾� �θ��� ��ġ�� ���� (���� ��ġ ������ ����)
            layer.parentTransform.localPosition = new Vector3(config.startPosition.x, config.startPosition.y, 0f);

            // ù ��° ��� ������Ʈ�� �������� ���� ���� ���
            SpriteRenderer referenceSR = layer.backgrounds[0].GetComponent<SpriteRenderer>();
            layer.backgroundWidth = referenceSR.size.x * config.scale.x;

            // ��� ��� ������Ʈ�� ��ġ�� ������ �缳��
            for (int i = 0; i < layer.backgrounds.Count; i++)
            {
                var bg = layer.backgrounds[i];
                bg.localScale = new Vector3(config.scale.x, config.scale.y, 1f); // ������ ����
                bg.localPosition = new Vector3(i * layer.backgroundWidth, 0f, 0f); // ���������� �̾������� ��ġ
            }

            // ����� �����ϰ� ���� ��ġ ���� (���� ������Ʈ �� �񱳿�)
            config.lastAppliedScale = config.scale;
            config.lastAppliedStartPosition = config.startPosition;
        }
    }


    // �� ������ ȣ��Ǿ� �з����� ȿ���� �����ϴ� �޼���
    void Update()
    {
        if (layers == null || playerTransform == null) return;

        // �÷��̾� ��ġ ��ȭ�� ���
        Vector3 deltaMovement = playerTransform.position - lastPlayerPosition;

        foreach (var layer in layers)
        {
            if (layer == null) continue;

            // ������ �� ��ġ ������Ʈ (���� ���� �� ��� �ݿ�)
            UpdateLayerProperties(layer);

            // ���̾� �̵� ���� (�з����� ����� ���� �ӵ� �ٸ�)
            layer.parentTransform.position += new Vector3(deltaMovement.x * layer.config.parallaxMultiplier, 0f, 0f);

            // ��� �ݺ� ó��
            float repeatDistance = layer.backgroundWidth * layer.config.imageCount; // ��ü ��� ����
            float halfRepeatDistance = repeatDistance / 2f; // �ݺ� ���� �Ÿ�

            foreach (var background in layer.backgrounds)
            {
                float distance = playerTransform.position.x - background.position.x; // �÷��̾�� ��� ���� �Ÿ� ���

                // �÷��̾ �ݺ� �Ÿ��� ���� �̻� �־��� ��� ����� �ݺ� ��ġ�� �̵�
                if (Mathf.Abs(distance) > halfRepeatDistance)
                {
                    background.position += new Vector3(repeatDistance * Mathf.Sign(distance), 0f, 0f); // ��� �ݺ� ��ġ�� �̵�
                }
            }
        }

        // �÷��̾� ��ġ ��� (���� �����ӿ��� �̵��� ��� �� ���)
        lastPlayerPosition = playerTransform.position;
    }
}
