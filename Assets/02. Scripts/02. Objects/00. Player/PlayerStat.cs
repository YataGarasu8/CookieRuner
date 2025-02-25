using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �÷��̾��� ���� ������ ����ϴ� Ŭ����
// PlayerStatsData(ScriptableObject)�κ��� �����͸� �޾ƿ� ���� ó�� �� ������ ���
public class PlayerStats : MonoBehaviour
{
    [Header("���� ������")]
    public PlayerStatsData statsData; // �÷��̾��� ���� ������ ���� ScriptableObject ����

    // ���� ü�� (���� ���� �� �ִ� ü������ �ʱ�ȭ)
    private float currentHealth;
    public float CurrentHealth { get { return currentHealth; } set { currentHealth = value; } }

    // �߰� �ӵ� (�������̳� ������ ���� ���� ����)
    private float speedModifier = 0f;

    // ���� ���� �ӵ� (�⺻ �ӵ� + �߰� �ӵ�)
    public float CurrentSpeed => statsData.baseSpeed + speedModifier;
    public bool isSpeedUP = false;

    private float currentScale;     // ���������� �����ϴ� ũ�� ��
    // ���� ũ�� ���� (ũ�� ����/���� �� ����)
    public float CurrentScale => Mathf.Max(currentScale, 0.01f); // �ּҰ� ����


    // ũ�� ��ȭ�� ���Ǵ� �ڷ�ƾ ���� (�ߺ� ���� ������)
    private Coroutine scaleCoroutine;

    // ���� ���� �� �ʱⰪ ����
    void Start()
    {
        if (statsData == null)
        {
            Debug.LogError("PlayerStatsData�� �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }

        currentHealth = statsData.maxHealth; // ü���� �ִ� ü������ �ʱ�ȭ
        currentScale = statsData.baseScale;  // �⺻ ũ�� ����
        UpdatePlayerScale();                 // Transform �����Ͽ� ����
    }

    // �÷��̾ �������� ���� �� ȣ��
    // ü���� 0 ���Ϸ� �������� �ʵ��� ó��
    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        Debug.Log($"Player Health: {currentHealth}");
    }

    // �÷��̾ ü���� ȸ���� �� ȣ��
    // �ִ� ü���� �ʰ����� �ʵ��� ó��
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, statsData.maxHealth);
    }

    // �߰� �ӵ��� ����/���� ��Ŵ
    // ����: ������ ȹ�� �� �ӵ� ����
    public void ModifySpeed(float amount)
    {
        isSpeedUP = true;
        speedModifier += amount;
    }

    // �߰� �ӵ��� �ʱ�ȭ (���� ȿ�� ���� �� ���)
    public void ResetSpeedModifier()
    {
        isSpeedUP = false;
        speedModifier = 0f;
    }

    // �÷��̾� ũ�⸦ ������ ������Ŵ
    // �ִ� ũ�⸦ �ʰ����� �ʵ��� ����
    public void IncreaseSize()
    {
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine); // ���� ũ�� ���� �ڷ�ƾ ����

        float targetScale = Mathf.Min(CurrentScale + statsData.scaleIncreaseAmount, statsData.maxScale);
        scaleCoroutine = StartCoroutine(ScaleOverTime(CurrentScale, targetScale));
    }

    // �÷��̾� ũ�⸦ �⺻ ũ��� �ǵ���
    public void ResetSize()
    {
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine); // ���� ũ�� ���� �ڷ�ƾ ����

        scaleCoroutine = StartCoroutine(ScaleOverTime(CurrentScale, statsData.baseScale));
    }

    // ũ�⸦ ���� �ð� ���� �ε巴�� ������
    // ��ȭ �Ϸ� �� ���� �ð��� ������ ������ ���� (targetScale�� baseScale�� �ƴ� ��)
    private System.Collections.IEnumerator ScaleOverTime(float startScale, float targetScale)
    {
        float elapsed = 0f;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * statsData.scaleChangeSpeed; // ũ�� ���� �ӵ� ����
            currentScale = Mathf.Lerp(startScale, targetScale, elapsed); // ���� �������� ũ�� ��ȭ
            UpdatePlayerScale(); // ���� ������Ʈ ũ�� �ݿ�
            yield return null;   // ������ ���
        }

        currentScale = targetScale; // ���� ũ�� ����
        UpdatePlayerScale();

        // ũ�� ���� �� ���� �ð� ���� �� ���� ����
        if (targetScale != statsData.baseScale)
        {
            yield return new WaitForSeconds(statsData.scaleDuration); // ���� �ð� ���
            ResetSize(); // ���� ����
        }
    }

    // ���� ũ�⸦ Transform�� localScale�� ����
    private void UpdatePlayerScale()
    {
        transform.localScale = Vector3.one * CurrentScale;
    }
}
