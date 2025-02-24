using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSC : MonoBehaviour
{

    Item data;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Rigidbody2D rb;

    [Header("Trigger ���� ����")]
    public Vector2 triggerSize = new Vector2(2f, 2f);
    public float triggerOffset = 20f;

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

    public void OnPlayerDetected()
    {
        Debug.Log("Trigger �������� �÷��̾� ����!");
        // ���� �� �߰� ���� ���� (��: �˾� ����, ���� �غ� ��)
    }
}
