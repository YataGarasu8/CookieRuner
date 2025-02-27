using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeLerp : MonoBehaviour
{
    [SerializeField] private float shakeMagnitude = 0.1f; // 흔들림 강도
    [SerializeField] private float shakeDuration = 0.5f;  // 흔들림 지속 시간
    [SerializeField] private float recoverySpeed = 5f;   // 원래 위치로 돌아가는 속도

    private Vector3 shakeOffset = Vector3.zero; // 흔들림 오프셋 저장
    private float currentShakeDuration = 0f;
    private Vector3 originalPosition; // 초기 카메라 위치

    public Vector3 ShakeOffset => shakeOffset; // 흔들림 값을 외부에서 가져올 수 있도록 설정

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

            // 흔들림 오프셋 설정 (기준이 되는 originalPosition을 유지)
            shakeOffset = new Vector3(x, y, 0);
            currentShakeDuration -= Time.deltaTime;
        }
        else
        {
            // 흔들림이 끝난 후 원래 위치로 점진적으로 복귀
            shakeOffset = Vector3.Lerp(shakeOffset, Vector3.zero, Time.deltaTime * recoverySpeed);
        }
    }
}
