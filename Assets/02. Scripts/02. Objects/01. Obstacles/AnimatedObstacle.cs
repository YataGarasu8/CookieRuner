using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways] // ������ ��忡���� ����
public class AnimatedObstacle : MonoBehaviour
{
    [Header("�ִϸ��̼� ����")]
    public string animationTriggerName = "isTrigger"; // �ִϸ��̼� Ʈ���� �̸�
    public int damage = 1; // �÷��̾� �浹 �� ������

    [Header("Trigger �浹ü ����(BoxCollider2D)")]
    public Transform triggerColliderObject;
    public Vector2 triggerColliderSize = new Vector2(2f, 2f);
    public Vector2 triggerColliderOffset = Vector2.zero;

    [Header("Damage �浹ü ����(PolygonCollider2D)")]
    public Transform damageColliderObject;
    public bool autoUpdateCollider = true; // ��������Ʈ ���� �� Collider �ڵ� ���� ����
    public bool useAnimationEvent = false; // Animation Event ��� ����

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
            Debug.LogError("Animator ������Ʈ�� ã�� �� �����ϴ�.");
        }

        InitializeColliders();
        UpdateTriggerColliderProperties();

        if (damagePolygonCollider != null)
        {
            damagePolygonCollider.enabled = false; // ���� �� ������ Collider ��Ȱ��ȭ
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        // ������ ��忡�� �ǽð� �浹ü �Ӽ� ������Ʈ
        if (!Application.isPlaying)
        {
            UpdateTriggerColliderProperties();
        }
        else if (autoUpdateCollider && damagePolygonCollider != null)
        {
            RefreshPolygonCollider(damagePolygonCollider); // ��������Ʈ ���� �� Collider �ڵ� ����
        }
    }
#endif

    private void InitializeColliders()
    {
        // Ʈ���� Collider ����
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
            Debug.LogError("TriggerColliderObject�� �������� �ʾҽ��ϴ�.");
        }

        // ������ PolygonCollider2D ����
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
                Debug.LogWarning("DamageColliderObject�� SpriteRenderer�� �����ϴ�. �ڵ� ������ �۵����� ���� �� �ֽ��ϴ�.");
            }

            damagePolygonCollider.isTrigger = false;
        }
        else
        {
            Debug.LogError("DamageColliderObject�� �������� �ʾҽ��ϴ�.");
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
            collider.enabled = true; // Collider ���� ����
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasPoppedUp && collision.CompareTag("Player"))
        {
            Debug.Log("�÷��̾ Ʈ���� �浹ü�� ����. �ִϸ��̼� Ʈ���� ����.");

            if (animator != null && HasAnimatorParameter(animationTriggerName, AnimatorControllerParameterType.Trigger))
            {
                animator.SetTrigger(animationTriggerName);
                hasPoppedUp = true;
            }
            else
            {
                Debug.LogWarning($"Animator�� '{animationTriggerName}' Ʈ���Ű� �������� �ʽ��ϴ�. Animator Controller�� Parameters�� Ȯ���ϼ���.");
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

    // �ִϸ��̼� �̺�Ʈ���� ȣ��: ������ Collider Ȱ��ȭ �� ����
    public void EnableDamageCollider()
    {
        if (damagePolygonCollider != null)
        {
            Debug.Log("�ִϸ��̼� �Ϸ�: ������ PolygonCollider Ȱ��ȭ.");
            damagePolygonCollider.enabled = true;

            if (useAnimationEvent)
            {
                RefreshPolygonCollider(damagePolygonCollider); // ���� ����
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (damagePolygonCollider != null && damagePolygonCollider.enabled && collision.collider.CompareTag("Player"))
        {
            Debug.Log($"�÷��̾�� �浹. ������ {damage} ����.");
            // �÷��̾� ������ ó�� ���� ���⿡ �߰� ����
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;

        // Ʈ���� Collider �ð�ȭ (���)
        if (triggerColliderObject != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(triggerColliderObject.localPosition + (Vector3)triggerColliderOffset, triggerColliderSize);
        }

        // ������ PolygonCollider �ð�ȭ (������)
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
