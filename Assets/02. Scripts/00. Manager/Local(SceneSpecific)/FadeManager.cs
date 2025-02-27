using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class FadeManager : MonoBehaviour
{
    [Header("페이드 설정")]
    [Tooltip("페이드에 사용될 UI 이미지 (전체 화면을 덮어야 함)")]
    public Image fadeImage; // 페이드 효과에 사용할 이미지

    [Tooltip("목표 알파 값 (기본: 0.5)")]
    [Range(0f, 1f)]
    public float targetAlpha = 0.5f; // 중간 단계 알파 값

    [Tooltip("페이드 전체 지속 시간 (초)")]
    public float fadeDuration = 2f; // 전체 페이드 시간 (인스펙터 적용 가능)

    public static FadeManager Instance { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!fadeImage)
        {
            Debug.LogError("FadeManager: fadeImage가 할당되지 않았습니다.");
            return;
        }

        fadeImage.gameObject.SetActive(false); // 초기에는 비활성화
        SetImageAlpha(0f); // 초기 알파 값 0 (완전 투명)
    }

    // 외부 호출용: 알파 값 0 -> targetAlpha -> 0 순으로 페이드 효과 적용
    public IEnumerator FadeInOut(Action onMiddleFade = null)
    {
        if (fadeDuration <= 0f)
        {
            Debug.LogWarning("FadeManager: fadeDuration 값이 0 이하입니다. 최소 0.1로 설정합니다.");
            fadeDuration = 0.1f; // 최소 지속 시간 보장
        }

        fadeImage.gameObject.SetActive(true); // 페이드 시작 시 활성화

        float halfDuration = fadeDuration / 2f; // 인스펙터에서 설정된 값을 사용

        // 1. 알파 값: 0 -> targetAlpha (화면 어두워짐)
        yield return StartCoroutine(FadeAlpha(0f, targetAlpha, halfDuration));

        // 중간 콜백 실행 (타일맵 교체 등)
        onMiddleFade?.Invoke();

        // 2. 알파 값: targetAlpha -> 0 (다시 밝아짐)
        yield return StartCoroutine(FadeAlpha(targetAlpha, 0f, halfDuration));

        fadeImage.gameObject.SetActive(false); // 페이드 종료 시 비활성화
    }

    // 알파 값을 선형적으로 변화시키는 코루틴
    private IEnumerator FadeAlpha(float startAlpha, float endAlpha, float duration)
    {
        if (duration <= 0f) yield break; // 지속 시간이 0 이하일 경우 즉시 반환

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            SetImageAlpha(Mathf.Lerp(startAlpha, endAlpha, t)); // 선형 보간
            yield return null;
        }

        SetImageAlpha(endAlpha); // 최종 알파 값 적용
    }

    // 이미지의 알파 값 설정
    private void SetImageAlpha(float alpha)
    {
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = alpha;
            fadeImage.color = color;
        }
    }
}
