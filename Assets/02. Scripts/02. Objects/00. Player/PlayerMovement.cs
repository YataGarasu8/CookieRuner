using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

// Update ��� FixedUpdate ���� �ۼ��ϴ� ������ ũ�� 4�����̴�.

// 1. ���� ������ �ϰ���  
//    - FixedUpdate�� ���� ���� ���� �޼���� ������ �ð� �������� ȣ��Ǳ� ������, ���� ���� �ڵ尡 �׻� ������ �ӵ��� ����  
//    - �ݸ� Update�� ������ �ӵ��� ���� ȣ�� �󵵰� �޶��� FPS ��ȭ �� �̵� �ӵ��� �޶��� �� ����
//    - ���� rb.velocity�� FixedUpdate���� �����ϸ� �����ӷ���Ʈ�� ������� ������ �̵� �ӵ��� �������� �̵��� ����  

// 2. ���� �������� ���ռ�  
//    - transform.position�� ���� �����ϸ� Unity�� ���� ������ �̵��� �ν����� ���� �浹�̳� �߷� ���� ���� ȿ���� ����
//    - �ݸ�, rb.velocity�� ����ϸ� Rigidbody�� ���� �̵����� ���� ������ �ڿ������� �����Ǿ� �浹 ó��, ������, �߷� ���� ���������� �۵��ϴ� ���� Ȯ����
//    - �� ����� Ŭ���̾�Ʈ������ ����, �������� ����Ƽ ������ ����� �� ������ Ȯ������ ����. Physics API�� ���������� �����ϰ� �ִ� ������ Ȯ�������� ����ϴ� ���� ������ �̾߱�

// 3. �ڵ� ������ �� ���� ����  
//    - Unity ���� ���������� ���� ������ FixedUpdate���� ó���� ���� �����ߴ�. �̷��� �ϰ� �� �������� ���
//      (https://docs.unity3d.com/kr/current/Manual/TimeFrameManagement.html)
//    - transform.position ���� ������ �������� ��ġ �̵��̳� �ڷ���Ʈ ���� Ư���� ��Ȳ������ ����ϴ� ���� �̻���  

// 4. �����ս��� ����ȭ  
//    - transform.position�� ����ϸ� ���� �������� ����ȭ �������� �߰� ���� ����� �߻��� �� ����  
//    - �ݴ�� rb.velocity�� ����ϸ� ���� ������ �̵��� ���� ó���ϹǷ� ���� ȿ���� ���� ������  
//    - �̰� ����Ƽ ��ü�� ����ϰԵ� �̻� ����� �������, �ȴٸ� ������� ������ ������ ���ϴ� �̷��� ��

// => ���  
//    - �������� �̵��̳� �浹 ó���� �ʿ��� ��Ȳ������ FixedUpdate���� rb.velocity�� ����ϴ� ���� ���� �������̰� ȿ����
//    - transform.position�� ���� ��ġ �̵��̳� �ڷ���Ʈó�� ���� ������ ������ ���� �ʴ� Ư�� ��Ȳ���� ����ϴ� ���� ����

// FixedUpdate �����ֱ� ���� ���� - https://docs.unity3d.com/kr/2019.4/Manual/ExecutionOrder.html




// �÷��̾� �̵�, ����, �����̵� �� �浹 ���� ������ �����ϴ� Ŭ����
// PlayerStats���� ���� �����͸� ������ �̵� �ӵ�, ũ�� � ����
// CircleCollider2D�� ����Ͽ� �����̵� �� �浹ü ������ �� ��ġ�� ����
[RequireComponent(typeof(Rigidbody2D), typeof(PlayerStats), typeof(BoxCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("�̵� �� ���� ����")]
    public float jumpForce = 10f;       // ���� �� ����� ��
    public int maxJumpCount = 2;        // �ִ� ���� Ƚ��

    [Header("�ٴ� üũ ����")]
    public Transform groundCheck;       // �ٴ� ������ ��ġ
    public float groundCheckRadius = 0.2f; // �ٴ� ���� �ݰ�
    public LayerMask groundLayer;       // ������ ���̾�

    [Header("�浹ü ���� (BoxCollider2D)")]
    public Vector2 normalColliderSize = new Vector2(1f, 2f);       // �Ϲ� ���� �浹ü ũ��
    public Vector2 normalColliderOffset = new Vector2(0f, -0.5f);  // �Ϲ� ���� ��ġ

    public Vector2 slideColliderSize = new Vector2(1.5f, 1f);      // �����̵� �� �浹ü ũ��
    public Vector2 slideColliderOffset = new Vector2(0f, -1f);     // �����̵� �� ��ġ

    private BoxCollider2D boxCollider; // �浹ü ����

    private Rigidbody2D rb;             // ���� ���� Rigidbody2D
    private Animator animator;          // �ִϸ��̼� ����� Animator
    private PlayerStats stats;          // �÷��̾� ���� ������ ����

    private int currentJumpCount = 0;   // ���� ���� Ƚ��
    private bool isGrounded = false;    // �ٴ� ���� ����
    private bool isSliding = false;     // �����̵� ����
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>();
        boxCollider = GetComponent<BoxCollider2D>();

        if (!(rb && animator && stats && boxCollider))
            Debug.LogError("�ʼ� ������Ʈ�� �����Ǿ����ϴ�.");
    }
    void Start()
    {
        animator.SetFloat(Constants.AnimatorParams.PlayerHP, stats.statsData.maxHealth);

        // stats �ʱ�ȭ �� ���� (ũ�� �ʱ�ȭ ����)
        Invoke(nameof(ApplyNormalCollider), 0.01f); // 0.01�� ���� ȣ��
    }

    void Update()
    {
        UpdateAnimatorParameters(); // �ִϸ����� �� ����
        HandleInput();              // �÷��̾� �Է� ó��
    }

    void FixedUpdate()
    {
        Move();                      // ���� �̵�
        isGrounded = CheckGrounded(); // �ٴ� ����

        // ���� �� ���� Ƚ�� �ʱ�ȭ
        if (isGrounded && currentJumpCount > 0)
            currentJumpCount = 0;
    }

    // �ν����Ϳ��� �� ���� �� �ڵ� ȣ�� (������ �ǽð� �ݿ�)
    void OnValidate()
    {
        // circleCollider�� �Ҵ���� �ʾ��� �� �ʱ�ȭ �õ�
        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider2D>();

        // �ʱ�ȭ ���� �� ��� �� ����
        if (boxCollider == null)
        {
            Debug.LogWarning("boxCollider2D �� �Ҵ���� �ʾҽ��ϴ�. �浹ü�� �߰��ϼ���.");
            return;
        }

        // ���¿� ���� �浹ü ����
        if (isSliding)
            ApplySlideCollider();
        else
            ApplyNormalCollider();
    }


    // ���������� ���� �̵� ó��
    private void Move()
    {
        rb.velocity = new Vector2(stats.CurrentSpeed, rb.velocity.y);
    }

    // �Է� ó��
    private void HandleInput()
    {
        // ���� �Է�
        if (Input.GetKeyDown(Constants.InputKeys.Jump))
            AttemptJump();

        // �����̵� ���� (�ٴڿ� ���� ���� ����)
        if (Input.GetKeyDown(Constants.InputKeys.Slide) && isGrounded)
        {
            isSliding = true;
            ApplySlideCollider();
        }

        // �����̵� ����
        if (Input.GetKeyUp(Constants.InputKeys.Slide))
        {
            isSliding = false;
            ApplyNormalCollider();
        }
    }

    // ���� �õ� ó��
    private void AttemptJump()
    {
        if (isSliding) return; // �����̵� �� ���� �Ұ�

        if (isGrounded)
            Jump();
        else if (currentJumpCount < maxJumpCount)
            DoubleJump();
    }

    // ���� ����
    public void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0); // ���� ���� �ӵ� ����
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // ���� �� ����
        animator.SetTrigger(Constants.AnimatorParams.JumpTrigger); // �ִϸ��̼� ����
        currentJumpCount = 1; // ���� Ƚ�� ����
    }

    // �̴� ���� ����
    public void DoubleJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0); // ���� ���� �ӵ� ����
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        animator.SetTrigger(Constants.AnimatorParams.DoubleJumpTrigger);
        currentJumpCount = 2;
    }

    // �ִϸ����� �Ķ���� ������Ʈ
    private void UpdateAnimatorParameters()
    {
        animator.SetFloat(Constants.AnimatorParams.Speed, Mathf.Abs(rb.velocity.x)); // ���� �ӵ� �ݿ�
        animator.SetFloat(Constants.AnimatorParams.VerticalVelocity, rb.velocity.y);  // ���� �ӵ� �ݿ�
        animator.SetBool(Constants.AnimatorParams.IsGrounded, isGrounded);            // �ٴ� ���� �ݿ�
        animator.SetBool(Constants.AnimatorParams.IsSliding, isSliding);              // �����̵� ���� �ݿ�
    }

    // �ٴ� ���� ���� Ȯ��
    private bool CheckGrounded()
    {
        Collider2D hit = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius * stats.CurrentScale, // �÷��̾� ũ�� �ݿ�
            groundLayer
        );

        return hit != null && hit.gameObject != gameObject;
    }

    // �Ϲ� ���� �浹ü ����
    private void ApplyNormalCollider()
    {
        if (boxCollider == null)
        {
            Debug.LogWarning("�浹ü�� null�Դϴ�.");
            return;
        }

        if (stats == null)
        {
            stats = GetComponent<PlayerStats>();
            if (stats == null)
            {
                Debug.LogWarning("PlayerStats�� null�Դϴ�.");
                return;
            }
        }

        if (boxCollider == null) return;
        boxCollider.size = normalColliderSize * stats.CurrentScale;  // ũ�� ����
        boxCollider.offset = normalColliderOffset * stats.CurrentScale; // ��ġ ����
    }

    // �����̵� �� �浹ü ���� (������ ���� �� ��ġ ����)
    private void ApplySlideCollider()
    {
        if (boxCollider == null)
        {
            Debug.LogWarning("�浹ü�� null�Դϴ�.");
            return;
        }

        if (stats == null)
        {
            stats = GetComponent<PlayerStats>();
            if (stats == null)
            {
                Debug.LogWarning("PlayerStats�� null�Դϴ�.");
                return;
            }
        }

        boxCollider.size = slideColliderSize * stats.CurrentScale;   // �����̵� ũ�� ����
        boxCollider.offset = slideColliderOffset * stats.CurrentScale; // �����̵� ��ġ ����
    }

    // �� â���� �浹 ���� �ð�ȭ
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red; // �ٴ� ���ο� ���� ����
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius * (stats != null ? stats.CurrentScale : 1f));
        }

        if (boxCollider != null)
        {
            Gizmos.color = isSliding ? Color.blue : Color.yellow; // �����̵� �� �Ķ�, �Ϲ� �� ���
            Vector2 colliderPosition = (Vector2)transform.position + boxCollider.offset;
            Gizmos.DrawWireCube(colliderPosition, boxCollider.size); // �ڽ� ���·� �ð�ȭ
        }
    }
}
