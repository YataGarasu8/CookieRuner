using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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






// �� Ʈ���Ÿ� FixedUpdate���� ȣ���ؾ� �ұ�?

// Unity���� �ִϸ��̼� Ʈ���� ȣ�� ������ �ִϸ��̼� ���� ��ȯ�� ���� ���ο� �߿��� ������ ��ħ
// Ư��, Ʈ���� ȣ���� Update���� ó���� �� �ִϸ��̼� ���� ��ȯ�� �Ҿ����ϰų� ���õǴ� ������ �߻��� �� ����!!!

// 1. Update�� Animator ������Ʈ Ÿ�̹� ����
// - Update�� �����Ӹ��� ȣ��Ǹ� ������ Ÿ�ְ̹� ����ȭ
// - FixedUpdate�� ���� ���� Ÿ�ְ̹� ����ȭ�Ǹ� ������ �������� ȣ��

// ���� �߻� �ó����� (Update ��� ��):
// - Update���� Ʈ���� ȣ�� �� ���� ������ ������ �ִϸ��̼� ���� ��ȯ ���� Ʈ���Ű� �����ǰų� ����� �߻�
// - �̷� ���� �ִϸ����Ͱ� Ʈ���� ���� ������ �������� ���ϰ� ��

// FixedUpdate ��� �� ����:
// - ���� ����� �ִϸ��̼� ó���� �ڿ������� ����ȭ
// - Ʈ���� ȣ�� �� �ִϸ��̼� ���� ��ȯ�� ���������� ����

// 2. Rigidbody���� �ϰ��� ����
// - ĳ������ �̵�, ����, �浹 ���� ���� ����(FixedUpdate)���� ó����
// - Ʈ���� ȣ���� FixedUpdate�� �θ� ������ �̵� �ִϸ��̼��� ���� ó���� �ϰ��ǰ� �����

// ����:
// - ���� �� ���������� ���� �̵��ϴ� Ÿ�ְ̹� �ִϸ��̼� ��� ������ ��ġ
// - �ִϸ��̼��� ������ �����Ӱ� �� �¾ƶ����� �� �ڿ������� ������ ����

// 3. Ʈ���� ȣ�� ������ ���� ���� ����
// - Update�� Animator ������Ʈ�� �浹�� ���, Ʈ���Ű� �ʹ� ������ ȣ�� �� �����Ǹ� �ִϸ��̼� ��ȯ ���� ���ɼ��� ����
// - FixedUpdate���� Ʈ���� ȣ�� ��, �ִϸ����� ������Ʈ ���� �����ϰ� Ʈ���Ÿ� ������ �� ����
// - Ʈ���� ���� ���� �� ���� Ÿ�̹� ��� ������

// ����:
// - Update�� �Է� ó���� ���������� Ʈ���� ���� Ÿ�̹��� �Ҿ����� �� ����
// - FixedUpdate�� ���� ����� ����ȭ�Ǿ� Ʈ���� ���� �������� Ȯ���� �� ����
// - ���� ��� ĳ���� ��Ʈ�ѷ������� Ʈ���� ȣ���� FixedUpdate���� ó���ϴ� ���� ������
// - �̸� ���� Ʈ���Ű� ������� �ʴ� ������ �����ϰ�, �ִϸ��̼� ��ȯ�� ���� ������ �ϰ����� ������ �� ����







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
    public Vector2 normalColliderOffset = new Vector2(0f, -0.5f);  // �Ϲ� ���� �浹ü ��ġ
    public Vector2 slideColliderSize = new Vector2(1.5f, 1f);      // �����̵� �� �浹ü ũ��
    public Vector2 slideColliderOffset = new Vector2(0f, -1f);     // �����̵� �� �浹ü ��ġ

    [Header("���� ����")]
    public float invincibilityDuration = 2f; // ���� ���� �ð�
    public float blinkInterval = 0.2f;       // ������ ����

    private Rigidbody2D rb;                  // ���� ���� Rigidbody2D
    private Animator animator;               // �ִϸ��̼� ����� Animator
    private PlayerStats stats;               // �÷��̾� ���� ������ ����
    private BoxCollider2D boxCollider;       // �Ϲ� �浹ü ����
    private BoxCollider2D itemCollider;       // ������ �浹ü ����
    private BoxCollider2D obsCollider;       // ��ֹ� �浹ü ����
    private SpriteRenderer spriteRenderer;   // ������ ȿ���� ���� ��������Ʈ ������

    private int currentJumpCount = 0;        // ���� ���� Ƚ��
    private bool isGrounded = false;         // �ٴ� ���� ����
    private bool isSliding = false;          // �����̵� ����
    private bool isInvincible = false;       // ���� ���� ����
    public bool isItemInvincible = false;       // ���� ������ ���� ����

    private bool shouldTriggerJump = false;         // ���� Ʈ���� ȣ�� �÷���
    private bool shouldTriggerDoubleJump = false;   // ���� ���� Ʈ���� ȣ�� �÷���

    private bool isGameOver = false;
    void Awake()
    {
        // �ʼ� ������Ʈ �ʱ�ȭ �� �˻�
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        itemCollider = GameObject.Find("ItemColider").GetComponent<BoxCollider2D>();
        obsCollider = GameObject.Find("ObsColider").GetComponent<BoxCollider2D>();

        GameManager.Instance.IsGameOver = false;

        // ������Ʈ�� ���� �� ���� �α� ���
        if (rb == null) Debug.LogError("[PlayerMovement] Rigidbody2D�� �����Ǿ����ϴ�.");
        if (animator == null) Debug.LogError("[PlayerMovement] Animator�� �����Ǿ����ϴ�.");
        if (stats == null) Debug.LogError("[PlayerMovement] PlayerStats�� �����Ǿ����ϴ�.");
        if (spriteRenderer == null) Debug.LogError("[PlayerMovement] SpriteRenderer�� �����Ǿ����ϴ�.");
        if (boxCollider == null) Debug.LogError("[PlayerMovement] BoxCollider2D�� �����Ǿ����ϴ�.");
        if (itemCollider == null) Debug.LogError("[PlayerMovement] itemCollider�� �����Ǿ����ϴ�.");
        if (obsCollider == null) Debug.LogError("[PlayerMovement] obsCollider�� �����Ǿ����ϴ�.");
    }

    void Start()
    {
        // �ִϸ����Ϳ� �ִ� ü�� �� ���� (���� �����Ͱ� ������ ���� ����)
        if (animator != null && stats != null && stats.statsData != null)
            animator.SetFloat(Constants.AnimatorParams.PlayerHP, stats.statsData.maxHealth);

        // �ʱ� �浹ü ���� (ũ�� �ʱ�ȭ ����)
        Invoke(nameof(ApplyNormalCollider), 0.01f); // 0.01�� ���� ȣ��� ���� ����

        
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.IsGameOver == false)
        {
            Move();                      // ���� �̵� ó��
            isGrounded = CheckGrounded(); // �ٴ� ���� ������Ʈ
            UpdateAnimatorParameters(); // �ִϸ����� �Ķ���� ������Ʈ

            // ���� Ʈ���� ȣ��
            if (shouldTriggerJump)
            {
                animator.SetTrigger(Constants.AnimatorParams.JumpTrigger);
                //Debug.Log("[JumpTrigger] Ʈ���� ȣ��");
                shouldTriggerJump = false;
            }

            if (shouldTriggerDoubleJump)
            {
                animator.SetTrigger(Constants.AnimatorParams.DoubleJumpTrigger);
                //Debug.Log("[DoubleJumpTrigger] Ʈ���� ȣ��");
                shouldTriggerDoubleJump = false;
            }

            // ���� �� ���� Ƚ�� �ʱ�ȭ
            if (isGrounded && currentJumpCount > 0)
                currentJumpCount = 0;

            if (stats.CurrentHealth <= 0.01)
            {
                GameOver();
            }

            if (this.gameObject.transform.position.y < -10f)
            {
                GameOver();
                SoundManager.Instance.PlaySFX("FallDeathSFX01");
            }

        }
    }

    void GameOver()
    {
        GameManager.Instance.IsGameOver = true;
        rb.velocity = new Vector2(0f, rb.velocity.y); // x�� �̵� �ӵ� ���� (y�� �ӵ��� ����)
    }

    // �ν����Ϳ��� �� ���� �� �ڵ� ȣ�� (������ �ǽð� �ݿ�)
    void OnValidate()
    {
        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider2D>();
        if(itemCollider == null)
            itemCollider = GetComponent<BoxCollider2D>();
        if (obsCollider == null)
            obsCollider = GetComponent<BoxCollider2D>();

        if (boxCollider == null)
        {
            Debug.LogWarning("[PlayerMovement] BoxCollider2D�� �Ҵ���� �ʾҽ��ϴ�. �浹ü�� �߰��ϼ���.");
            return;
        }

        if (itemCollider == null)
        {
            Debug.LogWarning("[PlayerMovement] itemCollider�� �Ҵ���� �ʾҽ��ϴ�. �浹ü�� �߰��ϼ���.");
            return;
        }

        if (obsCollider == null)
        {
            Debug.LogWarning("[PlayerMovement] itemCollider�� �Ҵ���� �ʾҽ��ϴ�. �浹ü�� �߰��ϼ���.");
            return;
        }

        // �����̵� ���¿� ���� �浹ü ����
        if (isSliding)
            ApplySlideCollider();
        else
            ApplyNormalCollider();
    }

    // ���������� ���� �̵� ó��
    private void Move()
    {
        if (rb != null && stats != null)
            rb.velocity = new Vector2(stats.CurrentSpeed, rb.velocity.y); // x�� �̵� �ӵ� ���� (y�� �ӵ��� ����)
    }

    // �Է� ó�� �޼���
    private void HandleInput()
    {
        // ���� �Է� ó��
        if (Input.GetKeyDown(Constants.InputKeys.Jump))
            AttemptJump();

        // �����̵� ���� (�ٴڿ� ���� ���� ����)
        if (Input.GetKeyDown(Constants.InputKeys.Slide) && isGrounded)
            StartSlide();

        // �����̵� ���� (�����̵� Ű ���� �� ����)
        if (Input.GetKeyUp(Constants.InputKeys.Slide))
            EndSlide();
    }

    // �����̵� ���� ó��
    public void StartSlide()
    {
        if (!isSliding && isGrounded)
        {
            isSliding = true;                     // �����̵� ���� Ȱ��ȭ
            ApplySlideCollider();                 // �����̵� �浹ü ����
            if (animator != null)
                animator.SetBool(Constants.AnimatorParams.IsSliding, true); // �ִϸ��̼� �����̵� ���� ����
        }
    }

    // �����̵� ���� ó��
    public void EndSlide()
    {
        if (isSliding)
        {
            isSliding = false;                    // �����̵� ���� ��Ȱ��ȭ
            ApplyNormalCollider();                // �Ϲ� �浹ü ����
            if (animator != null)
                animator.SetBool(Constants.AnimatorParams.IsSliding, false); // �ִϸ��̼� �����̵� ���� ����
        }
    }

    // ���� �õ� ó�� (�����̵� �߿��� ���� �Ұ�)
    public void AttemptJump()
    {
        if (isSliding) return; // �����̵� �� ���� �Ұ�

        if (isGrounded)
            Jump();
        else if (currentJumpCount < maxJumpCount)
            DoubleJump();
    }

    // ���� ���� (1�� ����)
    public void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0); // ���� ���� �ӵ� ����
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // �� �������� ������ ����
        animator.SetTrigger(Constants.AnimatorParams.JumpTrigger); // ���� �ִϸ��̼� ����
        shouldTriggerJump = true;
        currentJumpCount = 1; // ���� Ƚ�� ����
    }

    // �̴� ���� ����
    public void DoubleJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0); // ���� ���� �ӵ� ����
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // �̴� ������ ����
        animator.SetTrigger(Constants.AnimatorParams.DoubleJumpTrigger); // �̴� ���� �ִϸ��̼� ����
        shouldTriggerDoubleJump = true;
        currentJumpCount = 2; // ���� Ƚ�� ����
    }

    // �ִϸ����� �Ķ���� ������Ʈ
    private void UpdateAnimatorParameters()
    {
        animator.SetFloat(Constants.AnimatorParams.Speed, Mathf.Abs(rb.velocity.x));  // ���� �ӵ� �ݿ�
        animator.SetFloat(Constants.AnimatorParams.VerticalVelocity, rb.velocity.y);  // ���� �ӵ� �ݿ�
        animator.SetBool(Constants.AnimatorParams.IsGrounded, isGrounded);            // �ٴ� ���� ���� �ݿ�
        animator.SetBool(Constants.AnimatorParams.IsSliding, isSliding);              // �����̵� ���� �ݿ�
        animator.SetFloat(Constants.AnimatorParams.PlayerHP, stats.CurrentHealth); // ����ü�� �ݿ�
    }

    // �ٴ� ���� ���� Ȯ��
    private bool CheckGrounded()
    {
        Collider2D hit = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius * stats.CurrentScale, // �÷��̾� ũ�⿡ ���� ���� �ݰ� ����
            groundLayer
        );

        return hit != null && hit.gameObject != gameObject; // �ڱ� �ڽ� ����
    }

    // �Ϲ� ���� �浹ü ����
    private void ApplyNormalCollider()
    {

        if (boxCollider == null)
        {
            Debug.LogWarning("[PlayerMovement] BoxCollider2D�� null�Դϴ�. �浹ü ���� ����.");
            return;
        }

        if (itemCollider == null)
        {
            Debug.LogWarning("[PlayerMovement] itemCollider�� null�Դϴ�. �浹ü ���� ����.");
            return;
        }

        if (obsCollider == null)
        {
            Debug.LogWarning("[PlayerMovement] obsCollider�� null�Դϴ�. �浹ü ���� ����.");
            return;
        }

        if (stats == null)
        {
            stats = GetComponent<PlayerStats>();
            if (stats == null)
            {
                Debug.LogWarning("[PlayerMovement] PlayerStats�� null�Դϴ�. �浹ü ���� ����.");
                return;
            }
        }

        boxCollider.size = normalColliderSize;  // �Ϲ� ���� �浹ü ũ�� ����
        boxCollider.offset = normalColliderOffset; // �Ϲ� ���� �浹ü ��ġ ����
        itemCollider.size = normalColliderSize;  // �Ϲ� ���� �浹ü ũ�� ����
        itemCollider.offset = normalColliderOffset; // �Ϲ� ���� �浹ü ��ġ ����
        obsCollider.size = normalColliderSize;  // �Ϲ� ���� �浹ü ũ�� ����
        obsCollider.offset = normalColliderOffset; // �Ϲ� ���� �浹ü ��ġ ����
    }

    // �����̵� �� �浹ü ���� (ũ�� �� ��ġ ����)
    private void ApplySlideCollider()
    {
        if (boxCollider == null)
        {
            Debug.LogWarning("[PlayerMovement] BoxCollider2D�� null�Դϴ�. �浹ü ���� ����.");
            return;
        }

        if (itemCollider == null)
        {
            Debug.LogWarning("[PlayerMovement] itemCollider�� null�Դϴ�. �浹ü ���� ����.");
            return;
        }

        if (obsCollider == null)
        {
            Debug.LogWarning("[PlayerMovement] obsCollider�� null�Դϴ�. �浹ü ���� ����.");
            return;
        }

        if (stats == null)
        {
            stats = GetComponent<PlayerStats>();
            if (stats == null)
            {
                Debug.LogWarning("[PlayerMovement] PlayerStats�� null�Դϴ�. �浹ü ���� ����.");
                return;
            }
        }

        //slideColliderOffset = stats.isScaleUP ? new Vector2(0f, -0.4f) : new Vector2(0f, -1f);

        boxCollider.size = slideColliderSize;// �����̵� ���� �浹ü ũ�� ����
        boxCollider.offset = slideColliderOffset; // �����̵� ���� �浹ü ��ġ ����
        itemCollider.size = slideColliderSize;  // �����̵� ���� �浹ü ũ�� ����
        itemCollider.offset = slideColliderOffset; // �����̵� ���� �浹ü ��ġ ����
        obsCollider.size = slideColliderSize;   // �����̵� ���� �浹ü ũ�� ����
        obsCollider.offset = slideColliderOffset;// �����̵� ���� �浹ü ��ġ ����  * stats.CurrentScale; 

    }

    // �÷��̾� �ǰ� �� ȣ���� �Լ�
    public void TakeDamage(int damage)
    {
        if (isInvincible) return; // ���� ������ ���� ����

        // ü�� ���� ���� �߰� ����
        stats.TakeDamage(damage);

        StartCoroutine(InvincibilityCoroutine()); // ���� �� ������ �ڷ�ƾ ����
    }

    // ���� ���� �ڷ�ƾ
    private System.Collections.IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        obsCollider.gameObject.SetActive(false);
        float elapsedTime = 0f;

        while (elapsedTime < invincibilityDuration)
        {
            // �������ϰ� �����
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
            yield return new WaitForSeconds(blinkInterval);

            // ���� �������� ����
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(blinkInterval);

            elapsedTime += blinkInterval * 2;
        }

        obsCollider.gameObject.SetActive(true);
        isInvincible = false; // ���� ����
    }

    // �� â���� �浹 ���� �ð�ȭ (����� �뵵)
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red; // �ٴ� ���ο� ���� ����
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius * (stats != null ? stats.CurrentScale : 1f));
        }
        else
        {
            Debug.LogWarning("[PlayerMovement] groundCheck�� null�Դϴ�. Gizmos ǥ�� ����.");
        }

        if (boxCollider != null)
        {
            Gizmos.color = isSliding ? Color.blue : Color.yellow; // �����̵� �� �Ķ�, �Ϲ� �� ���
            Vector2 colliderPosition = (Vector2)transform.position + boxCollider.offset; // ���� ��ġ�� ������ �ջ�
            Gizmos.DrawWireCube(colliderPosition, boxCollider.size); // �浹ü �ð�ȭ
        }
        else
        {
            Debug.LogWarning("[PlayerMovement] BoxCollider2D�� null�Դϴ�. Gizmos ǥ�� ����.");
        }

        if (itemCollider != null)
        {
            Gizmos.color = isSliding ? Color.blue : Color.yellow; // �����̵� �� �Ķ�, �Ϲ� �� ���
            Vector2 colliderPosition = (Vector2)transform.position + itemCollider.offset; // ���� ��ġ�� ������ �ջ�
            Gizmos.DrawWireCube(colliderPosition, itemCollider.size); // �浹ü �ð�ȭ
        }
        else
        {
            Debug.LogWarning("[PlayerMovement] itemCollider2D�� null�Դϴ�. Gizmos ǥ�� ����.");
        }

        if (obsCollider != null)
        {
            Gizmos.color = isSliding ? Color.blue : Color.yellow; // �����̵� �� �Ķ�, �Ϲ� �� ���
            Vector2 colliderPosition = (Vector2)transform.position + obsCollider.offset; // ���� ��ġ�� ������ �ջ�
            Gizmos.DrawWireCube(colliderPosition, obsCollider.size); // �浹ü �ð�ȭ
        }
        else
        {
            Debug.LogWarning("[PlayerMovement] obsCollider2D�� null�Դϴ�. Gizmos ǥ�� ����.");
        }
    }

    public void TolggleImmune()
    {
        if (isInvincible)
        {
            isInvincible = false; isItemInvincible = false;
        }

        else
        {
            isInvincible = true; isItemInvincible = true;
        }
            
    }

    public void OnSpeedUPImmune()
    {
        obsCollider.gameObject.SetActive(false);
    }

    public void OffSpeedUPImmune()
    {
        obsCollider.gameObject.SetActive(true);
    }
}
