using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeLerp : MonoBehaviour
{
    [SerializeField] private float shakeMagnitude = 0.1f; // ��鸲 ����
    [SerializeField] private float shakeDuration = 0.5f;  // ��鸲 ���� �ð�
    [SerializeField] private float recoverySpeed = 5f;   // ���� ��ġ�� ���ư��� �ӵ�

    private Vector3 shakeOffset = Vector3.zero; // ��鸲 ������ ����
    private float currentShakeDuration = 0f;
    private Vector3 originalPosition; // �ʱ� ī�޶� ��ġ

    public Vector3 ShakeOffset => shakeOffset; // ��鸲 ���� �ܺο��� ������ �� �ֵ��� ����

    void Start()
    {
        originalPosition = transform.localPosition;
        shakeOffset = Vector3.zero;
    }

    public void Shake()
    {
        currentShakeDuration = shakeDuration;
    }

    public void Shake(float intensity, float duration)
    {
        shakeMagnitude = intensity;
        currentShakeDuration = duration;
    }

    void Update()
    {
        if (currentShakeDuration > 0)
        {
            float x = (Mathf.PerlinNoise(Time.time * 10, 0) * 2 - 1) * shakeMagnitude;
            float y = (Mathf.PerlinNoise(0, Time.time * 10) * 2 - 1) * shakeMagnitude;

            // ��鸲 ������ ���� (������ �Ǵ� originalPosition�� ����)
            shakeOffset = new Vector3(x, y, 0);
            currentShakeDuration -= Time.deltaTime;
        }
        else
        {
            // ��鸲�� ���� �� ���� ��ġ�� ���������� ����
            shakeOffset = Vector3.Lerp(shakeOffset, Vector3.zero, Time.deltaTime * recoverySpeed);
        }
    }
}
