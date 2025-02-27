using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeLerp : MonoBehaviour
{
    // ��鸲�� ������ �����ϴ� ����
    [SerializeField] private float shakeMagnitude = 0.1f;

    // ��鸲�� ���� �ð��� �����ϴ� ����
    [SerializeField] private float shakeDuration = 0.5f;

    // ���� ī�޶��� ��ġ�� �����ϴ� ����
    private Vector3 originalPosition;

    // ���� ��鸲�� ���� ���� �ð��� �����ϴ� ����
    private float currentShakeDuration = 0f;

    void Start()
    {
        // �ʱ� ���� �� ī�޶��� ���� ��ġ�� ����
        originalPosition = transform.localPosition;
    }

    // �⺻������ ������ ������ ���� �ð����� ī�޶� ���� �Լ�
    public void Shake()
    {
        currentShakeDuration = shakeDuration; // Inspector���� ������ ���� �ð����� ����
    }

    // ��鸲 ������ ���� �ð��� ���� �����Ͽ� �����ϴ� �Լ�
    public void Shake(float intensity, float duration)
    {
        shakeMagnitude = intensity; // �Ű������� ���� ������ ����
        currentShakeDuration = duration; // �Ű������� ���� ���� �ð����� ����
    }

    void Update()
    {
        // ��鸲 ���� �ð��� �����ִٸ� ��鸲 ����
        if (currentShakeDuration > 0)
        {
            // Perlin Noise�� ����Ͽ� ������ ���� ���� (�� �ڿ������� ��鸲 ����)
            float x = Mathf.PerlinNoise(Time.time * 10, 0) * 2 - 1;
            float y = Mathf.PerlinNoise(0, Time.time * 10) * 2 - 1;

            // ���� ��ġ���� ������ �������� �̵�
            Vector3 newPos = originalPosition + new Vector3(x, y, 0) * shakeMagnitude;

            // Lerp�� ����Ͽ� �ε巴�� ��鸲 ����
            transform.localPosition = Vector3.Lerp(transform.localPosition, newPos, Time.deltaTime * 10);

            // ��鸲 ���� �ð��� ����
            currentShakeDuration -= Time.deltaTime;
        }
        else
        {
            // ��鸲�� ������ ī�޶� ���� ��ġ�� �ǵ���
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * 10);
        }
    }
}
