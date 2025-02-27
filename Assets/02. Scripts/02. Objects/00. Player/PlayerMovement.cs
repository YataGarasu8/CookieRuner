using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

// Update 대신 FixedUpdate 에서 작성하는 이유는 크게 4가지이다.

// 1. 물리 연산의 일관성  
//    - FixedUpdate는 물리 연산 전용 메서드로 일정한 시간 간격으로 호출되기 때문에, 물리 관련 코드가 항상 일정한 속도로 실행  
//    - 반면 Update는 프레임 속도에 따라 호출 빈도가 달라져 FPS 변화 시 이동 속도가 달라질 수 있음
//    - 따라서 rb.velocity를 FixedUpdate에서 설정하면 프레임레이트와 상관없이 일정한 이동 속도와 안정적인 이동이 가능  

// 2. 물리 엔진과의 통합성  
//    - transform.position을 직접 변경하면 Unity의 물리 엔진이 이동을 인식하지 못해 충돌이나 중력 등의 물리 효과가 무시
//    - 반면, rb.velocity를 사용하면 Rigidbody를 통한 이동으로 물리 엔진과 자연스럽게 연동되어 충돌 처리, 마찰력, 중력 등이 정상적으로 작동하는 것을 확인함
//    - 이 방식은 클라이언트에서만 가능, 서버에선 유니티 물리를 사용할 수 있을지 확인하지 못함. Physics API를 내부적으로 포팅하고 있는 것으로 확인했으나 사용하는 것은 별개의 이야기

// 3. 코드 안정성 및 권장 사항  
//    - Unity 공식 문서에서도 물리 연산은 FixedUpdate에서 처리할 것을 권장했다. 이렇게 하게 된 결정적인 계기
//      (https://docs.unity3d.com/kr/current/Manual/TimeFrameManagement.html)
//    - transform.position 직접 변경은 순간적인 위치 이동이나 텔레포트 같은 특수한 상황에서만 사용하는 것이 이상적  

// 4. 퍼포먼스와 최적화  
//    - transform.position을 사용하면 물리 엔진과의 동기화 과정에서 추가 연산 비용이 발생할 수 있음  
//    - 반대로 rb.velocity를 사용하면 물리 엔진이 이동을 직접 처리하므로 연산 효율이 높고 안정적  
//    - 이건 유니티 강체를 사용하게된 이상 생기는 오버헤드, 싫다면 사용하지 않으면 되지만 편리하니 이렇게 함

// => 결론  
//    - 지속적인 이동이나 충돌 처리가 필요한 상황에서는 FixedUpdate에서 rb.velocity를 사용하는 것이 가장 안정적이고 효율적
//    - transform.position은 빠른 위치 이동이나 텔레포트처럼 물리 엔진의 영향을 받지 않는 특정 상황에만 사용하는 것이 좋음

// FixedUpdate 실행주기 관련 문서 - https://docs.unity3d.com/kr/2019.4/Manual/ExecutionOrder.html






// 왜 트리거를 FixedUpdate에서 호출해야 할까?

// Unity에서 애니메이션 트리거 호출 시점은 애니메이션 상태 전환의 성공 여부에 중요한 영향을 미침
// 특히, 트리거 호출을 Update에서 처리할 때 애니메이션 상태 전환이 불안정하거나 무시되는 현상이 발생할 수 있음!!!

// 1. Update와 Animator 업데이트 타이밍 차이
// - Update는 프레임마다 호출되며 렌더링 타이밍과 동기화
// - FixedUpdate는 물리 연산 타이밍과 동기화되며 일정한 간격으로 호출

// 문제 발생 시나리오 (Update 사용 시):
// - Update에서 트리거 호출 시 같은 프레임 내에서 애니메이션 상태 전환 전에 트리거가 해제되거나 덮어쓰기 발생
// - 이로 인해 애니메이터가 트리거 변경 사항을 감지하지 못하게 됨

// FixedUpdate 사용 시 장점:
// - 물리 연산과 애니메이션 처리가 자연스럽게 동기화
// - 트리거 호출 후 애니메이션 상태 전환이 안정적으로 적용

// 2. Rigidbody와의 일관성 유지
// - 캐릭터의 이동, 점프, 충돌 등은 물리 연산(FixedUpdate)에서 처리됨
// - 트리거 호출을 FixedUpdate에 두면 점프나 이동 애니메이션이 물리 처리와 일관되게 적용됨

// 예시:
// - 점프 시 물리적으로 위로 이동하는 타이밍과 애니메이션 재생 시점이 일치
// - 애니메이션이 물리적 움직임과 잘 맞아떨어져 더 자연스러운 연출이 가능

// 3. 트리거 호출 시점의 경쟁 조건 방지
// - Update와 Animator 업데이트가 충돌할 경우, 트리거가 너무 빠르게 호출 및 해제되면 애니메이션 전환 실패 가능성이 있음
// - FixedUpdate에서 트리거 호출 시, 애니메이터 업데이트 전에 안전하게 트리거를 적용할 수 있음
// - 트리거 상태 유지 및 해제 타이밍 제어가 쉬워짐

// 정리:
// - Update는 입력 처리에 적합하지만 트리거 적용 타이밍이 불안정할 수 있음
// - FixedUpdate는 물리 연산과 동기화되어 트리거 적용 안정성을 확보할 수 있음
// - 물리 기반 캐릭터 컨트롤러에서는 트리거 호출을 FixedUpdate에서 처리하는 것이 안정적
// - 이를 통해 트리거가 적용되지 않는 문제를 방지하고, 애니메이션 전환과 물리 연산의 일관성을 유지할 수 있음







// 플레이어 이동, 점프, 슬라이드 및 충돌 영역 변경을 관리하는 클래스
// PlayerStats에서 스탯 데이터를 가져와 이동 속도, 크기 등에 적용
// CircleCollider2D를 사용하여 슬라이드 시 충돌체 반지름 및 위치를 변경
[RequireComponent(typeof(Rigidbody2D), typeof(PlayerStats), typeof(BoxCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("이동 및 점프 설정")]
    public float jumpForce = 10f;       // 점프 시 적용될 힘
    public int maxJumpCount = 2;        // 최대 점프 횟수

    [Header("바닥 체크 설정")]
    public Transform groundCheck;       // 바닥 감지용 위치
    public float groundCheckRadius = 0.2f; // 바닥 감지 반경
    public LayerMask groundLayer;       // 감지할 레이어

    [Header("충돌체 설정 (BoxCollider2D)")]
    public Vector2 normalColliderSize = new Vector2(1f, 2f);       // 일반 상태 충돌체 크기
    public Vector2 normalColliderOffset = new Vector2(0f, -0.5f);  // 일반 상태 충돌체 위치
    public Vector2 slideColliderSize = new Vector2(1.5f, 1f);      // 슬라이드 시 충돌체 크기
    public Vector2 slideColliderOffset = new Vector2(0f, -1f);     // 슬라이드 시 충돌체 위치

    [Header("무적 설정")]
    public float invincibilityDuration = 2f; // 무적 지속 시간
    public float blinkInterval = 0.2f;       // 깜빡임 간격

    private Rigidbody2D rb;                  // 물리 계산용 Rigidbody2D
    private Animator animator;               // 애니메이션 제어용 Animator
    private PlayerStats stats;               // 플레이어 스탯 데이터 접근
    private BoxCollider2D boxCollider;       // 일반 충돌체 참조
    private BoxCollider2D itemCollider;       // 아이템 충돌체 참조
    private BoxCollider2D obsCollider;       // 장애물 충돌체 참조
    private SpriteRenderer spriteRenderer;   // 깜빡임 효과를 위한 스프라이트 렌더러

    private int currentJumpCount = 0;        // 현재 점프 횟수
    private bool isGrounded = false;         // 바닥 접촉 여부
    private bool isSliding = false;          // 슬라이드 상태
    private bool isInvincible = false;       // 무적 상태 여부
    public bool isItemInvincible = false;       // 무적 아이템 상태 여부

    private bool shouldTriggerJump = false;         // 점프 트리거 호출 플래그
    private bool shouldTriggerDoubleJump = false;   // 더블 점프 트리거 호출 플래그

    private bool isGameOver = false;
    void Awake()
    {
        // 필수 컴포넌트 초기화 및 검사
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        itemCollider = GameObject.Find("ItemColider").GetComponent<BoxCollider2D>();
        obsCollider = GameObject.Find("ObsColider").GetComponent<BoxCollider2D>();

        GameManager.Instance.IsGameOver = false;

        // 컴포넌트가 없을 시 오류 로그 출력
        if (rb == null) Debug.LogError("[PlayerMovement] Rigidbody2D가 누락되었습니다.");
        if (animator == null) Debug.LogError("[PlayerMovement] Animator가 누락되었습니다.");
        if (stats == null) Debug.LogError("[PlayerMovement] PlayerStats가 누락되었습니다.");
        if (spriteRenderer == null) Debug.LogError("[PlayerMovement] SpriteRenderer가 누락되었습니다.");
        if (boxCollider == null) Debug.LogError("[PlayerMovement] BoxCollider2D가 누락되었습니다.");
        if (itemCollider == null) Debug.LogError("[PlayerMovement] itemCollider가 누락되었습니다.");
        if (obsCollider == null) Debug.LogError("[PlayerMovement] obsCollider가 누락되었습니다.");
    }

    void Start()
    {
        // 애니메이터에 최대 체력 값 전달 (스탯 데이터가 존재할 때만 적용)
        if (animator != null && stats != null && stats.statsData != null)
            animator.SetFloat(Constants.AnimatorParams.PlayerHP, stats.statsData.maxHealth);

        // 초기 충돌체 설정 (크기 초기화 방지)
        Invoke(nameof(ApplyNormalCollider), 0.01f); // 0.01초 지연 호출로 적용 보장

        
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.IsGameOver == false)
        {
            Move();                      // 지속 이동 처리
            isGrounded = CheckGrounded(); // 바닥 감지 업데이트
            UpdateAnimatorParameters(); // 애니메이터 파라미터 업데이트

            // 점프 트리거 호출
            if (shouldTriggerJump)
            {
                animator.SetTrigger(Constants.AnimatorParams.JumpTrigger);
                //Debug.Log("[JumpTrigger] 트리거 호출");
                shouldTriggerJump = false;
            }

            if (shouldTriggerDoubleJump)
            {
                animator.SetTrigger(Constants.AnimatorParams.DoubleJumpTrigger);
                //Debug.Log("[DoubleJumpTrigger] 트리거 호출");
                shouldTriggerDoubleJump = false;
            }

            // 착지 시 점프 횟수 초기화
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
        rb.velocity = new Vector2(0f, rb.velocity.y); // x축 이동 속도 적용 (y축 속도는 유지)
    }

    // 인스펙터에서 값 변경 시 자동 호출 (에디터 실시간 반영)
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
            Debug.LogWarning("[PlayerMovement] BoxCollider2D가 할당되지 않았습니다. 충돌체를 추가하세요.");
            return;
        }

        if (itemCollider == null)
        {
            Debug.LogWarning("[PlayerMovement] itemCollider가 할당되지 않았습니다. 충돌체를 추가하세요.");
            return;
        }

        if (obsCollider == null)
        {
            Debug.LogWarning("[PlayerMovement] itemCollider가 할당되지 않았습니다. 충돌체를 추가하세요.");
            return;
        }

        // 슬라이드 상태에 따른 충돌체 적용
        if (isSliding)
            ApplySlideCollider();
        else
            ApplyNormalCollider();
    }

    // 오른쪽으로 지속 이동 처리
    private void Move()
    {
        if (rb != null && stats != null)
            rb.velocity = new Vector2(stats.CurrentSpeed, rb.velocity.y); // x축 이동 속도 적용 (y축 속도는 유지)
    }

    // 입력 처리 메서드
    private void HandleInput()
    {
        // 점프 입력 처리
        if (Input.GetKeyDown(Constants.InputKeys.Jump))
            AttemptJump();

        // 슬라이드 시작 (바닥에 있을 때만 가능)
        if (Input.GetKeyDown(Constants.InputKeys.Slide) && isGrounded)
            StartSlide();

        // 슬라이드 종료 (슬라이드 키 해제 시 적용)
        if (Input.GetKeyUp(Constants.InputKeys.Slide))
            EndSlide();
    }

    // 슬라이드 시작 처리
    public void StartSlide()
    {
        if (!isSliding && isGrounded)
        {
            isSliding = true;                     // 슬라이드 상태 활성화
            ApplySlideCollider();                 // 슬라이드 충돌체 적용
            if (animator != null)
                animator.SetBool(Constants.AnimatorParams.IsSliding, true); // 애니메이션 슬라이드 상태 적용
        }
    }

    // 슬라이드 종료 처리
    public void EndSlide()
    {
        if (isSliding)
        {
            isSliding = false;                    // 슬라이드 상태 비활성화
            ApplyNormalCollider();                // 일반 충돌체 복원
            if (animator != null)
                animator.SetBool(Constants.AnimatorParams.IsSliding, false); // 애니메이션 슬라이드 상태 해제
        }
    }

    // 점프 시도 처리 (슬라이드 중에는 점프 불가)
    public void AttemptJump()
    {
        if (isSliding) return; // 슬라이드 중 점프 불가

        if (isGrounded)
            Jump();
        else if (currentJumpCount < maxJumpCount)
            DoubleJump();
    }

    // 점프 실행 (1단 점프)
    public void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0); // 기존 수직 속도 제거
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // 위 방향으로 점프력 적용
        animator.SetTrigger(Constants.AnimatorParams.JumpTrigger); // 점프 애니메이션 실행
        shouldTriggerJump = true;
        currentJumpCount = 1; // 점프 횟수 갱신
    }

    // 이단 점프 실행
    public void DoubleJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0); // 기존 수직 속도 제거
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // 이단 점프력 적용
        animator.SetTrigger(Constants.AnimatorParams.DoubleJumpTrigger); // 이단 점프 애니메이션 실행
        shouldTriggerDoubleJump = true;
        currentJumpCount = 2; // 점프 횟수 갱신
    }

    // 애니메이터 파라미터 업데이트
    private void UpdateAnimatorParameters()
    {
        animator.SetFloat(Constants.AnimatorParams.Speed, Mathf.Abs(rb.velocity.x));  // 수평 속도 반영
        animator.SetFloat(Constants.AnimatorParams.VerticalVelocity, rb.velocity.y);  // 수직 속도 반영
        animator.SetBool(Constants.AnimatorParams.IsGrounded, isGrounded);            // 바닥 접촉 여부 반영
        animator.SetBool(Constants.AnimatorParams.IsSliding, isSliding);              // 슬라이드 상태 반영
        animator.SetFloat(Constants.AnimatorParams.PlayerHP, stats.CurrentHealth); // 현재체력 반영
    }

    // 바닥 접촉 여부 확인
    private bool CheckGrounded()
    {
        Collider2D hit = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius * stats.CurrentScale, // 플레이어 크기에 따른 감지 반경 적용
            groundLayer
        );

        return hit != null && hit.gameObject != gameObject; // 자기 자신 제외
    }

    // 일반 상태 충돌체 적용
    private void ApplyNormalCollider()
    {

        if (boxCollider == null)
        {
            Debug.LogWarning("[PlayerMovement] BoxCollider2D가 null입니다. 충돌체 적용 실패.");
            return;
        }

        if (itemCollider == null)
        {
            Debug.LogWarning("[PlayerMovement] itemCollider가 null입니다. 충돌체 적용 실패.");
            return;
        }

        if (obsCollider == null)
        {
            Debug.LogWarning("[PlayerMovement] obsCollider가 null입니다. 충돌체 적용 실패.");
            return;
        }

        if (stats == null)
        {
            stats = GetComponent<PlayerStats>();
            if (stats == null)
            {
                Debug.LogWarning("[PlayerMovement] PlayerStats가 null입니다. 충돌체 적용 실패.");
                return;
            }
        }

        boxCollider.size = normalColliderSize;  // 일반 상태 충돌체 크기 적용
        boxCollider.offset = normalColliderOffset; // 일반 상태 충돌체 위치 적용
        itemCollider.size = normalColliderSize;  // 일반 상태 충돌체 크기 적용
        itemCollider.offset = normalColliderOffset; // 일반 상태 충돌체 위치 적용
        obsCollider.size = normalColliderSize;  // 일반 상태 충돌체 크기 적용
        obsCollider.offset = normalColliderOffset; // 일반 상태 충돌체 위치 적용
    }

    // 슬라이드 시 충돌체 적용 (크기 및 위치 변경)
    private void ApplySlideCollider()
    {
        if (boxCollider == null)
        {
            Debug.LogWarning("[PlayerMovement] BoxCollider2D가 null입니다. 충돌체 적용 실패.");
            return;
        }

        if (itemCollider == null)
        {
            Debug.LogWarning("[PlayerMovement] itemCollider가 null입니다. 충돌체 적용 실패.");
            return;
        }

        if (obsCollider == null)
        {
            Debug.LogWarning("[PlayerMovement] obsCollider가 null입니다. 충돌체 적용 실패.");
            return;
        }

        if (stats == null)
        {
            stats = GetComponent<PlayerStats>();
            if (stats == null)
            {
                Debug.LogWarning("[PlayerMovement] PlayerStats가 null입니다. 충돌체 적용 실패.");
                return;
            }
        }

        //slideColliderOffset = stats.isScaleUP ? new Vector2(0f, -0.4f) : new Vector2(0f, -1f);

        boxCollider.size = slideColliderSize;// 슬라이드 상태 충돌체 크기 적용
        boxCollider.offset = slideColliderOffset; // 슬라이드 상태 충돌체 위치 적용
        itemCollider.size = slideColliderSize;  // 슬라이드 상태 충돌체 크기 적용
        itemCollider.offset = slideColliderOffset; // 슬라이드 상태 충돌체 위치 적용
        obsCollider.size = slideColliderSize;   // 슬라이드 상태 충돌체 크기 적용
        obsCollider.offset = slideColliderOffset;// 슬라이드 상태 충돌체 위치 적용  * stats.CurrentScale; 

    }

    // 플레이어 피격 시 호출할 함수
    public void TakeDamage(int damage)
    {
        if (isInvincible) return; // 무적 상태일 때는 무시

        // 체력 감소 로직 추가 예정
        stats.TakeDamage(damage);

        StartCoroutine(InvincibilityCoroutine()); // 무적 및 깜빡임 코루틴 실행
    }

    // 무적 상태 코루틴
    private System.Collections.IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        obsCollider.gameObject.SetActive(false);
        float elapsedTime = 0f;

        while (elapsedTime < invincibilityDuration)
        {
            // 반투명하게 만들기
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
            yield return new WaitForSeconds(blinkInterval);

            // 원래 색상으로 복원
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(blinkInterval);

            elapsedTime += blinkInterval * 2;
        }

        obsCollider.gameObject.SetActive(true);
        isInvincible = false; // 무적 해제
    }

    // 씬 창에서 충돌 영역 시각화 (디버깅 용도)
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red; // 바닥 여부에 따른 색상
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius * (stats != null ? stats.CurrentScale : 1f));
        }
        else
        {
            Debug.LogWarning("[PlayerMovement] groundCheck가 null입니다. Gizmos 표시 실패.");
        }

        if (boxCollider != null)
        {
            Gizmos.color = isSliding ? Color.blue : Color.yellow; // 슬라이드 시 파랑, 일반 시 노랑
            Vector2 colliderPosition = (Vector2)transform.position + boxCollider.offset; // 현재 위치와 오프셋 합산
            Gizmos.DrawWireCube(colliderPosition, boxCollider.size); // 충돌체 시각화
        }
        else
        {
            Debug.LogWarning("[PlayerMovement] BoxCollider2D가 null입니다. Gizmos 표시 실패.");
        }

        if (itemCollider != null)
        {
            Gizmos.color = isSliding ? Color.blue : Color.yellow; // 슬라이드 시 파랑, 일반 시 노랑
            Vector2 colliderPosition = (Vector2)transform.position + itemCollider.offset; // 현재 위치와 오프셋 합산
            Gizmos.DrawWireCube(colliderPosition, itemCollider.size); // 충돌체 시각화
        }
        else
        {
            Debug.LogWarning("[PlayerMovement] itemCollider2D가 null입니다. Gizmos 표시 실패.");
        }

        if (obsCollider != null)
        {
            Gizmos.color = isSliding ? Color.blue : Color.yellow; // 슬라이드 시 파랑, 일반 시 노랑
            Vector2 colliderPosition = (Vector2)transform.position + obsCollider.offset; // 현재 위치와 오프셋 합산
            Gizmos.DrawWireCube(colliderPosition, obsCollider.size); // 충돌체 시각화
        }
        else
        {
            Debug.LogWarning("[PlayerMovement] obsCollider2D가 null입니다. Gizmos 표시 실패.");
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
