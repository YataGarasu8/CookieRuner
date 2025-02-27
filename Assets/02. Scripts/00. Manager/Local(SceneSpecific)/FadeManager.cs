using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class FadeManager : MonoBehaviour
{
    // 페이드 효과에 사용될 UI 이미지 (전체 화면을 덮어야 함)
    public Image fadeImage;

    // 페이드 효과 중간에 도달할 목표 알파 값 (0~1, 여기서는 1을 사용하여 완전 불투명하게 함)
    public float targetAlpha = 1f;

    // 페이드 전체 지속 시간 (초)
    public float fadeDuration = 2f;

    // 싱글톤 인스턴스
    public static FadeManager Instance { get; private set; }

    void Awake()
    {
        // 이미 인스턴스가 존재하면 자신을 파괴하고 리턴
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // FadeManager는 씬 전환 시에도 파괴되지 않도록 함
        DontDestroyOnLoad(gameObject);

        // 씬 로드 시 호출될 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;

        // fadeImage가 할당되지 않았으면 에러 출력 후 리턴
        if (!fadeImage)
        {
            Debug.LogError("FadeManager: fadeImage가 할당되지 않았습니다.");
            return;
        }

        // 초기에는 fadeImage를 비활성화하고 알파 값을 0으로 설정 (완전 투명)
        fadeImage.gameObject.SetActive(false);
        SetImageAlpha(0f);
    }

    void OnDestroy()
    {
        // 씬 로드 이벤트 등록 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 로드될 때 호출됨. GameUI → Canvas → FadeImage 경로에서 fadeImage를 찾아 재할당하거나 없으면 새로 생성함.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // GameUI 오브젝트를 찾음
        GameObject gameUI = GameObject.Find("GameUI");
        if (gameUI != null)
        {
            // GameUI 하위의 Canvas 오브젝트를 찾음
            Transform canvasTransform = gameUI.transform.Find("Canvas");
            if (canvasTransform != null)
            {
                // Canvas 하위에서 FadeImage 오브젝트를 찾음
                Transform fadeImageTransform = canvasTransform.Find("FadeImage");
                if (fadeImageTransform != null)
                {
                    // 기존에 있는 FadeImage의 Image 컴포넌트를 가져와 fadeImage 변수에 할당
                    fadeImage = fadeImageTransform.GetComponent<Image>();
                    Debug.Log("FadeManager: 기존 FadeImage를 재할당했습니다.");
                }
                else
                {
                    Debug.LogWarning("Canvas 하위에서 FadeImage 오브젝트를 찾지 못했습니다. 새로 생성합니다.");

                    // FadeImage 오브젝트가 없으면 새로 생성
                    GameObject newFadeImageGO = new GameObject("FadeImage", typeof(Image));
                    // 새로 생성한 FadeImage를 Canvas의 자식으로 설정 (로컬 좌표 유지)
                    newFadeImageGO.transform.SetParent(canvasTransform, false);

                    // RectTransform을 이용해 화면 전체를 덮도록 설정
                    RectTransform rt = newFadeImageGO.GetComponent<RectTransform>();
                    rt.anchorMin = Vector2.zero;    // 좌측 하단 앵커 (0,0)
                    rt.anchorMax = Vector2.one;       // 우측 상단 앵커 (1,1)
                    rt.offsetMin = Vector2.zero;      // 왼쪽과 아래쪽 오프셋 0
                    rt.offsetMax = Vector2.zero;      // 오른쪽과 위쪽 오프셋 0
                    rt.pivot = new Vector2(0.5f, 0.5f); // 중앙 피봇

                    // 새로 생성한 Image 컴포넌트의 색상을 검정으로 설정 (알파 1)
                    Image newImage = newFadeImageGO.GetComponent<Image>();
                    newImage.color = Color.black;

                    // 새 Image 컴포넌트를 fadeImage 변수에 할당
                    fadeImage = newImage;
                    Debug.Log("FadeManager: 새 FadeImage 오브젝트를 생성하여 할당했습니다.");
                }

                // fadeImage 초기화: 비활성화하고 알파 값을 0으로 설정
                if (fadeImage != null)
                {
                    fadeImage.gameObject.SetActive(false);
                    SetImageAlpha(0f);
                }
                else
                {
                    Debug.LogError("FadeManager: fadeImage를 찾거나 생성하는데 실패했습니다.");
                }
            }
            else
            {
                Debug.LogError("GameUI 하위에서 Canvas 오브젝트를 찾지 못했습니다.");
            }
        }
        else
        {
            Debug.LogError("씬에서 GameUI 오브젝트를 찾지 못했습니다.");
        }
    }

    // 외부 호출용 코루틴: 알파 0에서 targetAlpha(1)까지 서서히 증가 후, 다시 1에서 0으로 감소함.
    public IEnumerator FadeInOut(Action onMiddleFade = null)
    {
        // fadeImage가 null이면 Canvas 오브젝트를 찾아 새 FadeImage를 생성
        if (fadeImage == null)
        {
            Debug.LogWarning("FadeManager: fadeImage가 null입니다. 새 FadeImage를 생성합니다.");
            GameObject canvasObj = GameObject.Find("Canvas");
            if (canvasObj == null)
            {
                Debug.LogError("FadeManager: Canvas 오브젝트를 찾을 수 없습니다. 페이드 효과를 진행할 수 없습니다.");
                yield break;
            }
            GameObject newFadeImageGO = new GameObject("FadeImage", typeof(Image));
            newFadeImageGO.transform.SetParent(canvasObj.transform, false);
            RectTransform rt = newFadeImageGO.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.pivot = new Vector2(0.5f, 0.5f);
            Image newImage = newFadeImageGO.GetComponent<Image>();
            newImage.color = Color.black;
            fadeImage = newImage;
            Debug.Log("FadeManager: 새 FadeImage 오브젝트를 생성하여 할당했습니다.");
        }

        // fadeDuration이 0 이하이면 최소 지속 시간 0.1초로 보정
        if (fadeDuration <= 0f)
        {
            Debug.LogWarning("FadeManager: fadeDuration 값이 0 이하입니다. 최소 0.1로 설정합니다.");
            fadeDuration = 0.1f;
        }

        // 페이드 효과 시작: fadeImage를 활성화함
        fadeImage.gameObject.SetActive(true);
        float halfDuration = fadeDuration / 2f;

        // 알파값 0에서 targetAlpha(1)로 서서히 증가시킴 (검정 화면으로 전환)
        yield return StartCoroutine(FadeAlpha(0f, targetAlpha, halfDuration));

        // 중간 콜백 실행 (예: 타일맵 전환 등의 처리)
        onMiddleFade?.Invoke();

        // 알파값 targetAlpha(1)에서 0으로 서서히 감소시킴 (검정에서 투명으로 전환)
        yield return StartCoroutine(FadeAlpha(targetAlpha, 0f, halfDuration));

        // 페이드 효과 종료: fadeImage를 비활성화함
        fadeImage.gameObject.SetActive(false);
    }

    // 알파 값을 선형 보간(Lerp)하여 변화시키는 코루틴
    private IEnumerator FadeAlpha(float startAlpha, float endAlpha, float duration)
    {
        if (duration <= 0f)
            yield break;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            SetImageAlpha(Mathf.Lerp(startAlpha, endAlpha, t));
            yield return null;
        }
        SetImageAlpha(endAlpha);
    }

    // fadeImage의 색상 알파값을 설정하는 함수
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
