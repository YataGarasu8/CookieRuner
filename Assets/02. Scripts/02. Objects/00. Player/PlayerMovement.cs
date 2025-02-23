using System.Collections;
using System.Collections.Generic;
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
    public Vector2 normalColliderOffset = new Vector2(0f, -0.5f);  // 일반 상태 위치

    public Vector2 slideColliderSize = new Vector2(1.5f, 1f);      // 슬라이드 시 충돌체 크기
    public Vector2 slideColliderOffset = new Vector2(0f, -1f);     // 슬라이드 시 위치

    private BoxCollider2D boxCollider; // 충돌체 참조

    private Rigidbody2D rb;             // 물리 계산용 Rigidbody2D
    private Animator animator;          // 애니메이션 제어용 Animator
    private PlayerStats stats;          // 플레이어 스탯 데이터 접근

    private int currentJumpCount = 0;   // 현재 점프 횟수
    private bool isGrounded = false;    // 바닥 접촉 여부
    private bool isSliding = false;     // 슬라이드 상태
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>();
        boxCollider = GetComponent<BoxCollider2D>();

        if (!(rb && animator && stats && boxCollider))
            Debug.LogError("필수 컴포넌트가 누락되었습니다.");
    }
    void Start()
    {
        animator.SetFloat(Constants.AnimatorParams.PlayerHP, stats.statsData.maxHealth);

        // stats 초기화 후 적용 (크기 초기화 방지)
        Invoke(nameof(ApplyNormalCollider), 0.01f); // 0.01초 지연 호출
    }

    void Update()
    {
        UpdateAnimatorParameters(); // 애니메이터 값 갱신
        HandleInput();              // 플레이어 입력 처리
    }

    void FixedUpdate()
    {
        Move();                      // 지속 이동
        isGrounded = CheckGrounded(); // 바닥 감지

        // 착지 시 점프 횟수 초기화
        if (isGrounded && currentJumpCount > 0)
            currentJumpCount = 0;
    }

    // 인스펙터에서 값 변경 시 자동 호출 (에디터 실시간 반영)
    void OnValidate()
    {
        // circleCollider가 할당되지 않았을 때 초기화 시도
        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider2D>();

        // 초기화 실패 시 경고 후 종료
        if (boxCollider == null)
        {
            Debug.LogWarning("boxCollider2D 가 할당되지 않았습니다. 충돌체를 추가하세요.");
            return;
        }

        // 상태에 따라 충돌체 적용
        if (isSliding)
            ApplySlideCollider();
        else
            ApplyNormalCollider();
    }


    // 오른쪽으로 지속 이동 처리
    private void Move()
    {
        rb.velocity = new Vector2(stats.CurrentSpeed, rb.velocity.y);
    }

    // 입력 처리
    private void HandleInput()
    {
        // 점프 입력
        if (Input.GetKeyDown(Constants.InputKeys.Jump))
            AttemptJump();

        // 슬라이드 시작 (바닥에 있을 때만 가능)
        if (Input.GetKeyDown(Constants.InputKeys.Slide) && isGrounded)
        {
            isSliding = true;
            ApplySlideCollider();
        }

        // 슬라이드 종료
        if (Input.GetKeyUp(Constants.InputKeys.Slide))
        {
            isSliding = false;
            ApplyNormalCollider();
        }
    }

    // 점프 시도 처리
    private void AttemptJump()
    {
        if (isSliding) return; // 슬라이드 중 점프 불가

        if (isGrounded)
            Jump();
        else if (currentJumpCount < maxJumpCount)
            DoubleJump();
    }

    // 점프 실행
    public void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0); // 기존 수직 속도 제거
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // 위로 힘 적용
        animator.SetTrigger(Constants.AnimatorParams.JumpTrigger); // 애니메이션 실행
        currentJumpCount = 1; // 점프 횟수 갱신
    }

    // 이단 점프 실행
    public void DoubleJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0); // 기존 수직 속도 제거
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        animator.SetTrigger(Constants.AnimatorParams.DoubleJumpTrigger);
        currentJumpCount = 2;
    }

    // 애니메이터 파라미터 업데이트
    private void UpdateAnimatorParameters()
    {
        animator.SetFloat(Constants.AnimatorParams.Speed, Mathf.Abs(rb.velocity.x)); // 수평 속도 반영
        animator.SetFloat(Constants.AnimatorParams.VerticalVelocity, rb.velocity.y);  // 수직 속도 반영
        animator.SetBool(Constants.AnimatorParams.IsGrounded, isGrounded);            // 바닥 여부 반영
        animator.SetBool(Constants.AnimatorParams.IsSliding, isSliding);              // 슬라이드 상태 반영
    }

    // 바닥 접촉 여부 확인
    private bool CheckGrounded()
    {
        Collider2D hit = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius * stats.CurrentScale, // 플레이어 크기 반영
            groundLayer
        );

        return hit != null && hit.gameObject != gameObject;
    }

    // 일반 상태 충돌체 적용
    private void ApplyNormalCollider()
    {
        if (boxCollider == null)
        {
            Debug.LogWarning("충돌체가 null입니다.");
            return;
        }

        if (stats == null)
        {
            stats = GetComponent<PlayerStats>();
            if (stats == null)
            {
                Debug.LogWarning("PlayerStats가 null입니다.");
                return;
            }
        }

        if (boxCollider == null) return;
        boxCollider.size = normalColliderSize * stats.CurrentScale;  // 크기 적용
        boxCollider.offset = normalColliderOffset * stats.CurrentScale; // 위치 적용
    }

    // 슬라이드 시 충돌체 적용 (반지름 감소 및 위치 변경)
    private void ApplySlideCollider()
    {
        if (boxCollider == null)
        {
            Debug.LogWarning("충돌체가 null입니다.");
            return;
        }

        if (stats == null)
        {
            stats = GetComponent<PlayerStats>();
            if (stats == null)
            {
                Debug.LogWarning("PlayerStats가 null입니다.");
                return;
            }
        }

        boxCollider.size = slideColliderSize * stats.CurrentScale;   // 슬라이드 크기 적용
        boxCollider.offset = slideColliderOffset * stats.CurrentScale; // 슬라이드 위치 적용
    }

    // 씬 창에서 충돌 영역 시각화
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red; // 바닥 여부에 따른 색상
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius * (stats != null ? stats.CurrentScale : 1f));
        }

        if (boxCollider != null)
        {
            Gizmos.color = isSliding ? Color.blue : Color.yellow; // 슬라이드 시 파랑, 일반 시 노랑
            Vector2 colliderPosition = (Vector2)transform.position + boxCollider.offset;
            Gizmos.DrawWireCube(colliderPosition, boxCollider.size); // 박스 형태로 시각화
        }
    }
}
