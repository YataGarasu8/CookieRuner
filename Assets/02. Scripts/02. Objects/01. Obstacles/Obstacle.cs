using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public ObstacleData data; // ScriptableObject 참조

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Rigidbody2D rb;

    [Header("Trigger 감지 설정")]
    public Vector2 triggerSize = new Vector2(2f, 2f);
    public float triggerOffset = 20f;

    private void Start()
    {
        InitializeComponents();
        SetupObstacle();
        SetupTriggerCollider(); // 트리거 콜라이더 추가
    }

    private void InitializeComponents()
    {
        // SpriteRenderer 설정
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        // Collider 설정
        var collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
        }
        collider.size = data.size;
        collider.isTrigger = false; // 충돌 감지를 위해 isTrigger 해제

        // Rigidbody2D 설정
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 0;
        rb.bodyType = RigidbodyType2D.Static; // 장애물은 고정 (움직이는 장애물은 코드 내에서 변경)
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
                Debug.LogWarning($"지원되지 않는 장애물 유형입니다: {data.type}");
                break;
        }
    }

    private void SetupStaticObstacle()
    {
        if (data.staticSprite != null)
            spriteRenderer.sprite = data.staticSprite;
        else
            Debug.LogWarning("Static 장애물에 스프라이트가 지정되지 않았습니다.");
    }

    private void SetupAnimatedObstacle()
    {
        if (data.animatorController == null)
        {
            Debug.LogError($"'{name}' 장애물에 AnimatorController가 지정되지 않았습니다.");
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
        rb.bodyType = RigidbodyType2D.Kinematic; // 이동 필요 → Kinematic 설정
        rb.velocity = new Vector2(-data.flyingSpeed, 0); // 왼쪽으로 이동
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
        Debug.Log("Trigger 영역에서 플레이어 감지!");
        // 감지 시 추가 로직 가능 (예: 팝업 시작, 공격 준비 등)
    }
}
