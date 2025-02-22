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
    [SerializeField] private GameObject loadingUI; // �ε� UI ��ü�� ���� �θ� ������Ʈ
    [SerializeField] private Slider progressBar; // ����� ǥ�� �����̴�
    [SerializeField] private TextMeshProUGUI progressText; // ����� �ۼ�Ʈ �ؽ�Ʈ
    [SerializeField] private TextMeshProUGUI loadingDetailText; // ���� �ε� �۾� �ؽ�Ʈ
    [SerializeField] private GameObject startButton; // �ε� �Ϸ� �� ǥ�õ� ���� ��ư

    [Header("Loading Settings")]
    [SerializeField] private string sceneToLoad; // �ε��� �� �̸�
    [SerializeField] private float buttonFloatAmplitude = 10f; // ��ư ������ ����
    [SerializeField] private float buttonFloatSpeed = 2f; // ��ư ������ �ӵ�
    [SerializeField] private float waitTimeAfterComplete = 1f; // �ε� �Ϸ� �� ��� �ð� (��)

    private ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>(); // ������ �� �޽��� ť
    private Vector3 startButtonInitialPosition; // ���� ��ư�� �ʱ� ��ġ
    private bool isLoading = true; // �ε� ���� �÷���

    // �ʱ� ���� �� �ε� ����
    void Start()
    {
        startButton.SetActive(false); // �ε� �� ��ư ����
        loadingUI.SetActive(true); // �ε� UI ǥ��
        startButtonInitialPosition = startButton.transform.localPosition; // ��ư �ʱ� ��ġ ����

        Thread loadingThread = new Thread(LoadResourcesInBackground); // ���ҽ� �ε� ������ ����
        loadingThread.Start();

        StartCoroutine(UpdateLoadingUI()); // UI ������Ʈ �ڷ�ƾ ����
    }

    // ��׶��忡�� �ε� �ùķ��̼� ó��
    private void LoadResourcesInBackground()
    {
        int totalSteps = 10; // �ӽ� �ε� �ܰ� ��
        float stepProgress = 0.7f / totalSteps; // ����� ���

        for (int i = 1; i <= totalSteps; i++)
        {
            Thread.Sleep(300); // �ε� �ùķ��̼�
            messageQueue.Enqueue("PROGRESS:" + stepProgress); // ����� �޽��� �߰�
        }

        messageQueue.Enqueue("Finalizing loading..."); // ���� �ܰ� �޽��� �߰�
        messageQueue.Enqueue("LOAD_SCENE"); // �� �ε� Ʈ���� �޽��� �߰�

        isLoading = false; // �ε� �Ϸ�
    }

    // ���� �����忡�� UI ������Ʈ �� �� �ε� ó��
    private IEnumerator UpdateLoadingUI()
    {
        float progress = 0f; // ����� �ʱ�ȭ

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

    // �� �񵿱� �ε� �� �ε� �Ϸ� ó��
    private IEnumerator LoadSceneAsync(float currentProgress)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad); // �� �ε� ����
        operation.allowSceneActivation = false; // �ڵ� ��ȯ ��Ȱ��ȭ

        float targetProgress = 1f; // ���� ��ǥ �����
        while (!operation.isDone)
        {
            float sceneProgress = Mathf.Clamp01(operation.progress / 0.9f); // �� �ε� ����� ���
            if (progressBar != null) progressBar.value = Mathf.Lerp(currentProgress, targetProgress, sceneProgress);
            if (progressText != null) progressText.text = $"{progressBar.value * 100:F0}%";

            if (operation.progress >= 0.9f)
            {
                if (progressText != null) progressText.text = "100%"; // ����� 100% ǥ��

                yield return new WaitForSeconds(waitTimeAfterComplete); // ������ ��� �ð���ŭ ���

                if (progressText != null) progressText.gameObject.SetActive(false); // ����� �ؽ�Ʈ ��Ȱ��ȭ
                if (loadingDetailText != null) loadingDetailText.gameObject.SetActive(false); // �ε� �� �ؽ�Ʈ ��Ȱ��ȭ
                if (progressBar != null) progressBar.gameObject.SetActive(false); // ����� �� ��Ȱ��ȭ

                loadingUI.SetActive(false); // ��ü �ε� UI ��Ȱ��ȭ
                startButton.SetActive(true); // ���� ��ư ǥ��
                StartCoroutine(FloatButton()); // ��ư ������ ����

                Button buttonComponent = startButton.GetComponent<Button>();
                buttonComponent.onClick.RemoveAllListeners();
                buttonComponent.onClick.AddListener(() => operation.allowSceneActivation = true); // Ŭ�� �� �� Ȱ��ȭ

                yield break;
            }
            yield return null;
        }
    }

    // ���� ��ư�� ���Ʒ� �ε巯�� ������ ó��
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