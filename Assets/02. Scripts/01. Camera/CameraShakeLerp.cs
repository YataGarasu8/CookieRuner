using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeLerp : MonoBehaviour
{
    // 흔들림의 강도를 조절하는 변수
    [SerializeField] private float shakeMagnitude = 0.1f;

    // 흔들림의 지속 시간을 조절하는 변수
    [SerializeField] private float shakeDuration = 0.5f;

    // 원래 카메라의 위치를 저장하는 변수
    private Vector3 originalPosition;

    // 현재 흔들림이 진행 중인 시간을 추적하는 변수
    private float currentShakeDuration = 0f;

    void Start()
    {
        // 초기 시작 시 카메라의 원래 위치를 저장
        originalPosition = transform.localPosition;
    }

    // 기본적으로 설정된 강도와 지속 시간으로 카메라를 흔드는 함수
    public void Shake()
    {
        currentShakeDuration = shakeDuration; // Inspector에서 설정된 지속 시간으로 적용
    }

    // 흔들림 강도와 지속 시간을 직접 설정하여 실행하는 함수
    public void Shake(float intensity, float duration)
    {
        shakeMagnitude = intensity; // 매개변수로 받은 강도로 설정
        currentShakeDuration = duration; // 매개변수로 받은 지속 시간으로 설정
    }

    void Update()
    {
        // 흔들림 지속 시간이 남아있다면 흔들림 실행
        if (currentShakeDuration > 0)
        {
            // Perlin Noise를 사용하여 랜덤한 값을 생성 (더 자연스러운 흔들림 구현)
            float x = Mathf.PerlinNoise(Time.time * 10, 0) * 2 - 1;
            float y = Mathf.PerlinNoise(0, Time.time * 10) * 2 - 1;

            // 원래 위치에서 랜덤한 방향으로 이동
            Vector3 newPos = originalPosition + new Vector3(x, y, 0) * shakeMagnitude;

            // Lerp를 사용하여 부드럽게 흔들림 적용
            transform.localPosition = Vector3.Lerp(transform.localPosition, newPos, Time.deltaTime * 10);

            // 흔들림 지속 시간을 감소
            currentShakeDuration -= Time.deltaTime;
        }
        else
        {
            // 흔들림이 끝나면 카메라를 원래 위치로 되돌림
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * 10);
        }
    }
}
