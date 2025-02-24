using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Ÿ�ϸ��� �����ϸ� �÷��̾� �̵��� ���� Ÿ�ϸ� ���� �� ���Ÿ� ����ϴ� Ŭ����
// - �ν����� â���� ���� Ÿ�ϸ� �������� �޾� ������� ���
// - Ȱ��ȭ�� Ÿ�ϸ� ������ 2���� ����
// - ���� �÷��̾ ��ġ�� Ÿ�ϸ��� ���̸� ������ �ش� ��ġ�� �̾ ���� Ÿ�ϸ� ����
// - �÷��̾ Ÿ�ϸ��� 3���� 2 ������ ������ ���ο� Ÿ�ϸ� ���� �� ���� Ÿ�ϸ� ����
public class TilemapManager : MonoBehaviour
{
    [Header("Ÿ�ϸ� ���� ����")]
    [Tooltip("����� Ÿ�ϸ� ������ ����Ʈ (���� ������� �߰�)")]
    public List<GameObject> tilemapPrefabs;       // ����� Ÿ�ϸ� ������ ����Ʈ

    [Tooltip("�÷��̾� Ʈ������")]
    public Transform player;                      // �÷��̾� ����

    private Queue<GameObject> activeTilemaps = new Queue<GameObject>(); // Ȱ�� Ÿ�ϸ� ť (�ִ� 2�� ����)
    private int currentTilemapIndex = 0;                                // ���� ����� Ÿ�ϸ� �ε���
    private GameObject currentTilemap;                                  // ���� �÷��̾ ��ġ�� Ÿ�ϸ�
    private float currentTilemapThresholdX;                             // ���� Ÿ�ϸ��� 3���� 2 ���� X ��ǥ

    void Start()
    {
        if (tilemapPrefabs == null || tilemapPrefabs.Count == 0)
        {
            Debug.LogError("TilemapManager: Ÿ�ϸ� ������ ����Ʈ�� ��� �ֽ��ϴ�. �ν����Ϳ��� �����ϼ���.");
            return;
        }

        if (!player)
        {
            Debug.LogError("TilemapManager: �÷��̾ �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }

        // �ʱ� Ÿ�ϸ� ���� �� �Ӱ� ���� ����
        SpawnInitialTilemap();
        UpdateTilemapThreshold();
    }

    void Update()
    {
        if (player.position.x >= currentTilemapThresholdX)
        {
            SpawnTilemap();           // ���� Ÿ�ϸ� ����
            UpdateTilemapThreshold(); // ���ο� Ÿ�ϸ� �Ӱ谪 ����

            // �ִ� Ȱ�� Ÿ�ϸ� ��(2��) �ʰ� �� ���� Ÿ�ϸ� ����
            if (activeTilemaps.Count > 2)
            {
                RemoveOldestTilemap();
            }
        }
    }

    // �ʱ� Ÿ�ϸ� ���� �� ť�� �߰�
    private void SpawnInitialTilemap()
    {
        GameObject initialTilemap = Instantiate(GetNextTilemapPrefab(), Vector3.zero, Quaternion.identity);
        initialTilemap.transform.SetParent(transform); // TilemapManager ������ ��ġ
        activeTilemaps.Enqueue(initialTilemap);
        currentTilemap = initialTilemap;

        // parallaxLayer ã�� �� �÷��̾� transform �Ҵ�
        var parallaxLayer = currentTilemap.GetComponentInChildren<MultiLayerParallaxWithDynamicStartPositionAndScale>();
        if (parallaxLayer != null)
        {
            parallaxLayer.playerTransform = player;
        }
        else
        {
            Debug.LogWarning("parallaxLayer�� ã�� �� �����ϴ�.");
        }
    }

    // ���ο� Ÿ�ϸ� ���� �� ť�� �߰�
    private void SpawnTilemap()
    {
        float tilemapLength = GetTilemapWorldLength(currentTilemap);
        Vector3 spawnPosition = currentTilemap.transform.position + new Vector3(tilemapLength, 0f, 0f);

        GameObject newTilemap = Instantiate(GetNextTilemapPrefab(), spawnPosition, Quaternion.identity);
        newTilemap.transform.SetParent(transform); // TilemapManager ������ ��ġ

        // parallaxLayer ã�� �� �÷��̾� transform �Ҵ�
        var parallaxLayer = newTilemap.GetComponentInChildren<MultiLayerParallaxWithDynamicStartPositionAndScale>();
        if (parallaxLayer != null)
        {
            parallaxLayer.playerTransform = player;
        }
        else
        {
            Debug.LogWarning("parallaxLayer�� ã�� �� �����ϴ�.");
        }

        activeTilemaps.Enqueue(newTilemap); // ť�� �߰�
        currentTilemap = newTilemap;        // ���� Ÿ�ϸ� ������Ʈ
    }

    // Ÿ�ϸ� ������ ����Ʈ���� ���� Ÿ�ϸ� �������� (��ȯ)
    private GameObject GetNextTilemapPrefab()
    {
        GameObject prefab = tilemapPrefabs[currentTilemapIndex];
        currentTilemapIndex = (currentTilemapIndex + 1) % tilemapPrefabs.Count; // �ε��� ��ȯ
        return prefab;
    }

    // ���� Ÿ�ϸ��� 3���� 2 ���� ��� �� ������Ʈ
    private void UpdateTilemapThreshold()
    {
        float tilemapLength = GetTilemapWorldLength(currentTilemap);
        currentTilemapThresholdX = currentTilemap.transform.position.x + (tilemapLength * 2f / 3f);
    }

    // ���� ������ Ÿ�ϸ� ����
    private void RemoveOldestTilemap()
    {
        if (activeTilemaps.Count == 0) return;

        GameObject oldestTilemap = activeTilemaps.Dequeue();
        if (oldestTilemap != null)
        {
            Destroy(oldestTilemap); // �޸� ������ ���� ����
        }
    }

    // Ÿ�ϸ��� ���� ���̸� ���� ��ǥ �������� ���
    private float GetTilemapWorldLength(GameObject tilemapObj)
    {
        Tilemap tilemap = tilemapObj.GetComponentInChildren<Tilemap>();

        if (tilemap == null)
        {
            Debug.LogWarning("TilemapManager: Tilemap ������Ʈ�� ã�� �� �����ϴ�. �⺻ ���� 10 ����.");
            return 10f; // �⺻ ���� ����
        }

        tilemap.CompressBounds(); // Ÿ�ϸ� ��� ����ȭ
        Bounds tilemapBounds = tilemap.localBounds; // ���� ��� ��������

        // ���� ��踦 ���� ������ ��ȯ
        Vector3 worldSize = Vector3.Scale(tilemapBounds.size, tilemap.transform.lossyScale);
        return worldSize.x;
    }
}
