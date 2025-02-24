using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Concurrent;
using System.Threading;

public class LoadingSceneController : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject loadingUI; // 로딩 UI 전체를 묶은 부모 오브젝트
    [SerializeField] private Slider progressBar; // 진행률 표시 슬라이더
    [SerializeField] private TextMeshProUGUI progressText; // 진행률 퍼센트 텍스트
    [SerializeField] private TextMeshProUGUI loadingDetailText; // 현재 로딩 작업 텍스트
    [SerializeField] private GameObject startButton; // 로딩 완료 후 표시될 시작 버튼

    [Header("Loading Settings")]
    [SerializeField] private string sceneToLoad; // 로드할 씬 이름
    [SerializeField] private float buttonFloatAmplitude = 10f; // 버튼 움직임 범위
    [SerializeField] private float buttonFloatSpeed = 2f; // 버튼 움직임 속도
    [SerializeField] private float waitTimeAfterComplete = 1f; // 로딩 완료 후 대기 시간 (초)

    private ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>(); // 스레드 간 메시지 큐
    private Vector3 startButtonInitialPosition; // 시작 버튼의 초기 위치
    private bool isLoading = true; // 로딩 상태 플래그

    // 초기 설정 및 로딩 시작
    void Start()
    {
        startButton.SetActive(false); // 로딩 중 버튼 숨김
        loadingUI.SetActive(true); // 로딩 UI 표시
        startButtonInitialPosition = startButton.transform.localPosition; // 버튼 초기 위치 저장

        Thread loadingThread = new Thread(LoadResourcesInBackground); // 리소스 로드 스레드 시작
        loadingThread.Start();

        StartCoroutine(UpdateLoadingUI()); // UI 업데이트 코루틴 시작
    }

    // 백그라운드에서 로딩 시뮬레이션 처리
    private void LoadResourcesInBackground()
    {
        int totalSteps = 10; // 임시 로딩 단계 수
        float stepProgress = 0.7f / totalSteps; // 진행률 계산

        for (int i = 1; i <= totalSteps; i++)
        {
            Thread.Sleep(300); // 로딩 시뮬레이션
            messageQueue.Enqueue("PROGRESS:" + stepProgress); // 진행률 메시지 추가
        }

        messageQueue.Enqueue("Finalizing loading..."); // 최종 단계 메시지 추가
        messageQueue.Enqueue("LOAD_SCENE"); // 씬 로드 트리거 메시지 추가

        isLoading = false; // 로딩 완료
    }

    // 메인 스레드에서 UI 업데이트 및 씬 로드 처리
    private IEnumerator UpdateLoadingUI()
    {
        float progress = 0f; // 진행률 초기화

        while (isLoading || !messageQueue.IsEmpty)
        {
            while (messageQueue.TryDequeue(out string message))
            {
                if (message.StartsWith("PROGRESS:"))
                {
                    float increment = float.Parse(message.Replace("PROGRESS:", ""));
                    progress = Mathf.Clamp01(progress + increment);
                    if (progressBar != null) progressBar.value = progress;
                    if (progressText != null) progressText.text = $"{progress * 100:F0}%";
                }
                else if (message == "LOAD_SCENE")
                {
                    yield return StartCoroutine(LoadSceneAsync(progress));
                }
            }
            yield return null;
        }
    }

    // 씬 비동기 로드 및 로딩 완료 처리
    private IEnumerator LoadSceneAsync(float currentProgress)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad); // 씬 로드 시작
        operation.allowSceneActivation = false; // 자동 전환 비활성화

        float targetProgress = 1f; // 최종 목표 진행률
        while (!operation.isDone)
        {
            float sceneProgress = Mathf.Clamp01(operation.progress / 0.9f); // 씬 로드 진행률 계산
            if (progressBar != null) progressBar.value = Mathf.Lerp(currentProgress, targetProgress, sceneProgress);
            if (progressText != null) progressText.text = $"{progressBar.value * 100:F0}%";

            if (operation.progress >= 0.9f)
            {
                if (progressText != null) progressText.text = "100%"; // 진행률 100% 표시

                yield return new WaitForSeconds(waitTimeAfterComplete); // 설정된 대기 시간만큼 대기

                if (progressText != null) progressText.gameObject.SetActive(false); // 진행률 텍스트 비활성화
                if (loadingDetailText != null) loadingDetailText.gameObject.SetActive(false); // 로딩 상세 텍스트 비활성화
                if (progressBar != null) progressBar.gameObject.SetActive(false); // 진행률 바 비활성화

                loadingUI.SetActive(false); // 전체 로딩 UI 비활성화
                startButton.SetActive(true); // 시작 버튼 표시
                StartCoroutine(FloatButton()); // 버튼 움직임 시작

                Button buttonComponent = startButton.GetComponent<Button>();
                buttonComponent.onClick.RemoveAllListeners();
                buttonComponent.onClick.AddListener(() => operation.allowSceneActivation = true); // 클릭 시 씬 활성화

                yield break;
            }
            yield return null;
        }
    }

    // 시작 버튼의 위아래 부드러운 움직임 처리
    private IEnumerator FloatButton()
    {
        while (startButton.activeSelf)
        {
            float newY = startButtonInitialPosition.y + Mathf.Sin(Time.time * buttonFloatSpeed) * buttonFloatAmplitude;
            startButton.transform.localPosition = new Vector3(startButtonInitialPosition.x, newY, startButtonInitialPosition.z);
            yield return null;
        }
    }
}