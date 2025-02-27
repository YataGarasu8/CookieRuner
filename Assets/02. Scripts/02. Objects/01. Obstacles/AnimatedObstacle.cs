using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways] // 에디터 모드에서도 실행
public class AnimatedObstacle : MonoBehaviour
{
    [Header("애니메이션 설정")]
    public string animationTriggerName = "isTrigger"; // 애니메이션 트리거 이름
    public int damage = 1; // 플레이어 충돌 시 데미지

    [Header("Trigger 충돌체 설정(BoxCollider2D)")]
    public Transform triggerColliderObject;
    public Vector2 triggerColliderSize = new Vector2(2f, 2f);
    public Vector2 triggerColliderOffset = Vector2.zero;

    [Header("Damage 충돌체 설정(PolygonCollider2D)")]
    public Transform damageColliderObject;
    public bool autoUpdateCollider = true; // 스프라이트 변경 시 Collider 자동 갱신 여부
    public bool useAnimationEvent = false; // Animation Event 사용 여부

    private Animator animator;
    private BoxCollider2D triggerCollider;
    private PolygonCollider2D damagePolygonCollider;
    private SpriteRenderer spriteRenderer;
    private bool hasPoppedUp = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator 컴포넌트를 찾을 수 없습니다.");
        }

        InitializeColliders();
        UpdateTriggerColliderProperties();

        if (damagePolygonCollider != null)
        {
            damagePolygonCollider.enabled = false; // 시작 시 데미지 Collider 비활성화
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        // 에디터 모드에서 실시간 충돌체 속성 업데이트
        if (!Application.isPlaying)
        {
            UpdateTriggerColliderProperties();
        }
        else if (autoUpdateCollider && damagePolygonCollider != null)
        {
            RefreshPolygonCollider(damagePolygonCollider); // 스프라이트 변경 시 Collider 자동 갱신
        }
    }
#endif

    private void InitializeColliders()
    {
        // 트리거 Collider 설정
        if (triggerColliderObject != null)
        {
            triggerCollider = triggerColliderObject.GetComponent<BoxCollider2D>();
            if (triggerCollider == null)
            {
                triggerCollider = triggerColliderObject.gameObject.AddComponent<BoxCollider2D>();
            }
            triggerCollider.isTrigger = true;
        }
        else
        {
            Debug.LogError("TriggerColliderObject가 지정되지 않았습니다.");
        }

        // 데미지 PolygonCollider2D 설정
        if (damageColliderObject != null)
        {
            damagePolygonCollider = damageColliderObject.GetComponent<PolygonCollider2D>();
            spriteRenderer = damageColliderObject.GetComponent<SpriteRenderer>();

            if (damagePolygonCollider == null)
            {
                damagePolygonCollider = damageColliderObject.gameObject.AddComponent<PolygonCollider2D>();
            }

            if (spriteRenderer == null)
            {
                Debug.LogWarning("DamageColliderObject에 SpriteRenderer가 없습니다. 자동 갱신이 작동하지 않을 수 있습니다.");
            }

            damagePolygonCollider.isTrigger = false;
        }
        else
        {
            Debug.LogError("DamageColliderObject가 지정되지 않았습니다.");
        }
    }

    private void UpdateTriggerColliderProperties()
    {
        if (triggerCollider != null)
        {
            triggerCollider.size = triggerColliderSize;
            triggerCollider.offset = triggerColliderOffset;
        }
    }

    private void RefreshPolygonCollider(PolygonCollider2D collider)
    {
        if (collider != null)
        {
            collider.enabled = false;
            collider.enabled = true; // Collider 강제 갱신
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasPoppedUp && collision.CompareTag("Player"))
        {
            Debug.Log("플레이어가 트리거 충돌체에 접촉. 애니메이션 트리거 실행.");

            if (animator != null && HasAnimatorParameter(animationTriggerName, AnimatorControllerParameterType.Trigger))
            {
                animator.SetTrigger(animationTriggerName);
                hasPoppedUp = true;
            }
            else
            {
                Debug.LogWarning($"Animator에 '{animationTriggerName}' 트리거가 존재하지 않습니다. Animator Controller의 Parameters를 확인하세요.");
            }
        }
    }

    private bool HasAnimatorParameter(string paramName, AnimatorControllerParameterType type)
    {
        foreach (var param in animator.parameters)
        {
            if (param.name == paramName && param.type == type)
                return true;
        }
        return false;
    }

    // 애니메이션 이벤트에서 호출: 데미지 Collider 활성화 및 갱신
    public void EnableDamageCollider()
    {
        if (damagePolygonCollider != null)
        {
            Debug.Log("애니메이션 완료: 데미지 PolygonCollider 활성화.");
            damagePolygonCollider.enabled = true;

            if (useAnimationEvent)
            {
                RefreshPolygonCollider(damagePolygonCollider); // 강제 갱신
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (damagePolygonCollider != null && damagePolygonCollider.enabled && collision.collider.CompareTag("Player"))
        {
            Debug.Log($"플레이어와 충돌. 데미지 {damage} 적용.");
            // 플레이어 데미지 처리 로직 여기에 추가 예정
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;

        // 트리거 Collider 시각화 (녹색)
        if (triggerColliderObject != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(triggerColliderObject.localPosition + (Vector3)triggerColliderOffset, triggerColliderSize);
        }

        // 데미지 PolygonCollider 시각화 (빨간색)
        if (damageColliderObject != null && damagePolygonCollider != null && damagePolygonCollider.pathCount > 0)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < damagePolygonCollider.pathCount; i++)
            {
                var points = damagePolygonCollider.GetPath(i);
                for (int j = 0; j < points.Length; j++)
                {
                    Vector3 start = damageColliderObject.TransformPoint(points[j]);
                    Vector3 end = damageColliderObject.TransformPoint(points[(j + 1) % points.Length]);
                    Gizmos.DrawLine(start, end);
                }
            }
        }
    }
}
