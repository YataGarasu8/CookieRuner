using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public ObstacleData data; // ScriptableObject ����

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Rigidbody2D rb;

    [Header("Trigger ���� ����")]
    public Vector2 triggerSize = new Vector2(2f, 2f);
    public float triggerOffset = 20f;

    private void Start()
    {
        InitializeComponents();
        SetupObstacle();
        SetupTriggerCollider(); // Ʈ���� �ݶ��̴� �߰�
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
        collider.isTrigger = false; // �浹 ������ ���� isTrigger ����

        // Rigidbody2D ����
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 0;
        rb.bodyType = RigidbodyType2D.Static; // ��ֹ��� ���� (�����̴� ��ֹ��� �ڵ� ������ ����)
    }

    private void SetupObstacle()
    {
        switch (data.type)
        {
            case ObstacleActionType.Static:
                SetupStaticObstacle();
                break;

            case ObstacleActionType.FlyToPlayer:
                SetupFlyingObstacle();
                break;

            case ObstacleActionType.PopUp:
            case ObstacleActionType.Disappear:
                SetupAnimatedObstacle();
                break;

            default:
                Debug.LogWarning($"�������� �ʴ� ��ֹ� �����Դϴ�: {data.type}");
                break;
        }
    }

    private void SetupStaticObstacle()
    {
        if (data.staticSprite != null)
            spriteRenderer.sprite = data.staticSprite;
        else
            Debug.LogWarning("Static ��ֹ��� ��������Ʈ�� �������� �ʾҽ��ϴ�.");
    }

    private void SetupAnimatedObstacle()
    {
        if (data.animatorController == null)
        {
            Debug.LogError($"'{name}' ��ֹ��� AnimatorController�� �������� �ʾҽ��ϴ�.");
            return;
        }

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = gameObject.AddComponent<Animator>();
        }

        animator.runtimeAnimatorController = data.animatorController;
    }

    private void SetupFlyingObstacle()
    {
        rb.bodyType = RigidbodyType2D.Kinematic; // �̵� �ʿ� �� Kinematic ����
        rb.velocity = new Vector2(-data.flyingSpeed, 0); // �������� �̵�
    }

    private void SetupTriggerCollider()
    {
        GameObject triggerArea = new GameObject("TriggerArea");
        triggerArea.transform.parent = transform;
        triggerArea.transform.localPosition = new Vector3(-triggerOffset, 0, 0);

        BoxCollider2D triggerCollider = triggerArea.AddComponent<BoxCollider2D>();
        triggerCollider.isTrigger = true;
        triggerCollider.size = triggerSize;

        TriggerDetector detector = triggerArea.AddComponent<TriggerDetector>();
        detector.SetObstacle(this);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("ObsColider"))
        {
            //PlayerStats playerStats = collision.collider.GetComponentInParent<PlayerStats>();
            PlayerController playerController = collision.collider.GetComponentInParent<PlayerController>();
            PlayerMovement playerMovement = collision.collider.GetComponentInParent<PlayerMovement>();
            if (playerController != null)
            {
                if (playerMovement.isItemInvincible == false)
                {
                    playerController.OnHit(data.damage);
                }
                else
                {
                    rb.bodyType = RigidbodyType2D.Dynamic;
                }
            }
        }
    }

    public void OnPlayerDetected()
    {
        Debug.Log("Trigger �������� �÷��̾� ����!");
        // ���� �� �߰� ���� ���� (��: �˾� ����, ���� �غ� ��)
    }
}
