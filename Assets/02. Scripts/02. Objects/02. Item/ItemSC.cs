using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSC : MonoBehaviour
{

    public Item data;
    private SpriteRenderer spriteRenderer;
    //private Animator animator;
    private Rigidbody2D rb;

    [Header("Trigger ���� ����")]
    public Vector2 triggerSize = new Vector2(2f, 2f);
    public float triggerOffset = 5f;

    private void Start()
    {
        InitializeComponents();
        SetupItem();
        SetupTriggerCollider(); // Ʈ���� �ݶ��̴� �߰�
    }

    private void SetupItem()
    {
        if (data.icon != null)
            spriteRenderer.sprite = data.icon;
        else
            Debug.LogWarning("�����ۿ� ��������Ʈ�� �������� �ʾҽ��ϴ�.");
    }
    private void InitializeComponents()
    {
        // SpriteRenderer ����
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        // Collider ����
        var collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
        }
        collider.size = data.size;
        collider.isTrigger = true;

        // Rigidbody2D ����
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 0;
        rb.bodyType = RigidbodyType2D.Static; // ��ֹ��� ���� (�����̴� ��ֹ��� �ڵ� ������ ����)
    }

    private void SetupTriggerCollider()
    {
        GameObject triggerArea = new GameObject("TriggerArea");
        triggerArea.transform.parent = transform;
        triggerArea.transform.localPosition = new Vector3(-triggerOffset, 0, 0);

        BoxCollider2D triggerCollider = triggerArea.AddComponent<BoxCollider2D>();
        triggerCollider.isTrigger = true;
        triggerCollider.size = triggerSize;

        ItemTrriger trriger = triggerArea.AddComponent<ItemTrriger>();
        trriger.SetItem(this);
    }

    public void OnPlayerDetected(Collider2D collider)
    {
        PlayerStats playerStats = collider.GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.Log("������Ʈ ����");
            return;
        }
        Debug.Log("Trigger �������� �÷��̾� ����!");
        // ���� �� �߰� ���� ���� (��: �˾� ����, ���� �غ� ��)
        switch (data.Type)
        {
            case ItemType.Score:
                ScoreManager.Instance.AddScore(data.score);
                break;
            case ItemType.HPUPItem:
                playerStats.Heal(data.healthBonus);
                break;
            case ItemType.SpeedUPItem:
                playerStats.ModifySpeed(data.speedBonus);
                break;
            case ItemType.TreasureItem:
                {
                    playerStats.Heal(data.healthBonus);
                    playerStats.ModifySpeed(data.speedBonus);
                }
                break;
            case ItemType.BonusItem:
                //���ʽ������� ó��
                break;
            default:
                Debug.Log("����Ʈ");
                break;
        }
    }
}
