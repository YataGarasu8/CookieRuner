using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerStats))]
public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;  // 이동 및 행동 처리 담당
    private PlayerStats stats;        // 플레이어 스탯 관리

    [Header("펫 설정")]
    public Pet currentPet;            // 플레이어가 데리고 있는 펫 (선택 사항)

    void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        stats = GetComponent<PlayerStats>();

        if (movement == null) Debug.LogError("[PlayerController] PlayerMovement가 누락되었습니다.");
        if (stats == null) Debug.LogError("[PlayerController] PlayerStats가 누락되었습니다.");
    }

    void Start()
    {
        if (currentPet != null)
        {
            PetController petController = currentPet.GetComponent<PetController>();
            if (petController != null)
                petController.player = transform;
            else
                Debug.LogWarning("[PlayerController] PetController가 Pet에 없습니다.");
        }
    }

    void Update()
    {
        HandleMovementInput();  // 점프 및 슬라이드 입력 처리
        HandleSpeedInput();     // 속도 조절 입력 처리
        HandleSizeInput();      // 크기 조절 입력 처리
        //Debug.Log($"{stats.CurrentHealth}");
        HandleDamageTestInput(); // 임시 데미지 입력 처리
    }

    // 이동 관련 입력 처리
    private void HandleMovementInput()
    {
        // 점프 입력 처리
        if (Input.GetKeyDown(Constants.InputKeys.Jump))
            movement.AttemptJump();

        // 슬라이드 시작 (바닥에 있을 때만 가능)
        if (Input.GetKeyDown(Constants.InputKeys.Slide))
            movement.StartSlide();

        // 슬라이드 종료
        if (Input.GetKeyUp(Constants.InputKeys.Slide))
            movement.EndSlide();
    }

    // 속도 조절 입력 처리
    private void HandleSpeedInput()
    {
        if (Input.GetKeyDown(Constants.InputKeys.IncreaseSpeed))
            stats.ModifySpeed(1f);  // 속도 증가

        if (Input.GetKeyDown(Constants.InputKeys.DecreaseSpeed))
            stats.ModifySpeed(-1f); // 속도 감소
    }

    // 크기 조절 입력 처리
    private void HandleSizeInput()
    {
        if (Input.GetKeyDown(Constants.InputKeys.IncreaseSize))
            stats.IncreaseSize();  // 크기 증가

        if (Input.GetKeyDown(Constants.InputKeys.ResetSize))
            stats.ResetSize();     // 크기 초기화
    }

    // 외부에서 피격 시 호출 (예: 적의 공격 스크립트에서 호출)
    public void OnHit(int damage)
    {
        movement.TakeDamage(damage);
    }
    
    // 임시 데미지 입력 처리 (C 키 입력 시 10 데미지 적용)
    private void HandleDamageTestInput()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            int testDamage = 10;
            Debug.Log($"[PlayerController] C 키 입력으로 {testDamage} 데미지 적용");
            OnHit(testDamage);
        }
    }
}
