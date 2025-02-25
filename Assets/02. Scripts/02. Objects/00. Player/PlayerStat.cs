using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어의 스탯 로직을 담당하는 클래스
// PlayerStatsData(ScriptableObject)로부터 데이터를 받아와 로직 처리 및 적용을 담당
public class PlayerStats : MonoBehaviour
{
    [Header("스탯 데이터")]
    public PlayerStatsData statsData; // 플레이어의 스탯 정보를 담은 ScriptableObject 참조

    // 현재 체력 (게임 시작 시 최대 체력으로 초기화)
    private float currentHealth;
    public float CurrentHealth { get { return currentHealth; } set { currentHealth = value; } }

    // 추가 속도 (아이템이나 버프에 의해 변동 가능)
    private float speedModifier = 0f;

    // 현재 적용 속도 (기본 속도 + 추가 속도)
    public float CurrentSpeed => statsData.baseSpeed + speedModifier;
    public bool isSpeedUP = false;

    private float currentScale;     // 내부적으로 관리하는 크기 값
    // 현재 크기 상태 (크기 증가/감소 시 변경)
    public float CurrentScale => Mathf.Max(currentScale, 0.01f); // 최소값 보장


    // 크기 변화에 사용되는 코루틴 참조 (중복 실행 방지용)
    private Coroutine scaleCoroutine;

    // 게임 시작 시 초기값 설정
    void Start()
    {
        if (statsData == null)
        {
            Debug.LogError("PlayerStatsData가 할당되지 않았습니다.");
            return;
        }

        currentHealth = statsData.maxHealth; // 체력을 최대 체력으로 초기화
        currentScale = statsData.baseScale;  // 기본 크기 설정
        UpdatePlayerScale();                 // Transform 스케일에 적용
    }

    // 플레이어가 데미지를 받을 때 호출
    // 체력이 0 이하로 떨어지지 않도록 처리
    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        Debug.Log($"Player Health: {currentHealth}");
    }

    // 플레이어가 체력을 회복할 때 호출
    // 최대 체력을 초과하지 않도록 처리
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, statsData.maxHealth);
    }

    // 추가 속도를 증가/감소 시킴
    // 예시: 아이템 획득 시 속도 증가
    public void ModifySpeed(float amount)
    {
        isSpeedUP = true;
        speedModifier += amount;
    }

    // 추가 속도를 초기화 (버프 효과 종료 시 사용)
    public void ResetSpeedModifier()
    {
        isSpeedUP = false;
        speedModifier = 0f;
    }

    // 플레이어 크기를 일정량 증가시킴
    // 최대 크기를 초과하지 않도록 제한
    public void IncreaseSize()
    {
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine); // 기존 크기 변경 코루틴 중지

        float targetScale = Mathf.Min(CurrentScale + statsData.scaleIncreaseAmount, statsData.maxScale);
        scaleCoroutine = StartCoroutine(ScaleOverTime(CurrentScale, targetScale));
    }

    // 플레이어 크기를 기본 크기로 되돌림
    public void ResetSize()
    {
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine); // 기존 크기 변경 코루틴 중지

        scaleCoroutine = StartCoroutine(ScaleOverTime(CurrentScale, statsData.baseScale));
    }

    // 크기를 일정 시간 동안 부드럽게 변경함
    // 변화 완료 후 일정 시간이 지나면 원상태 복귀 (targetScale이 baseScale이 아닐 때)
    private System.Collections.IEnumerator ScaleOverTime(float startScale, float targetScale)
    {
        float elapsed = 0f;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * statsData.scaleChangeSpeed; // 크기 변경 속도 적용
            currentScale = Mathf.Lerp(startScale, targetScale, elapsed); // 선형 보간으로 크기 변화
            UpdatePlayerScale(); // 실제 오브젝트 크기 반영
            yield return null;   // 프레임 대기
        }

        currentScale = targetScale; // 최종 크기 적용
        UpdatePlayerScale();

        // 크기 증가 시 일정 시간 유지 후 원상 복귀
        if (targetScale != statsData.baseScale)
        {
            yield return new WaitForSeconds(statsData.scaleDuration); // 유지 시간 대기
            ResetSize(); // 원상 복귀
        }
    }

    // 현재 크기를 Transform의 localScale에 적용
    private void UpdatePlayerScale()
    {
        transform.localScale = Vector3.one * CurrentScale;
    }
}
