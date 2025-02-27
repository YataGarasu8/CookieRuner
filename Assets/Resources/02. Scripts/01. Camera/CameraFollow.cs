using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject targetObject;
    private CameraShakeLerp cameraShake; // CameraShakeLerp 스크립트 참조

    [Header("카메라 오프셋 설정")]
    public float offsetX = 0f;
    public float offsetY = 0f;

    private Vector3 initialOffset;
    private Vector3 originalPosition; // 원래 카메라 위치

    void Start()
    {
        if (targetObject == null)
        {
            Debug.LogError("타겟 오브젝트가 할당되지 않았습니다.");
            return;
        }

        // 초기 오프셋 계산
        initialOffset = targetObject.transform.position - transform.position;
        offsetX += initialOffset.x;

        // CameraShakeLerp 참조 가져오기
        cameraShake = GetComponent<CameraShakeLerp>();

        // 초기 카메라 위치 저장
        originalPosition = transform.position;
    }

    void LateUpdate() // 흔들림 적용 후 카메라 위치 조정
    {
        if (targetObject == null) return;

        // 기본적으로 플레이어를 따라가는 위치 계산
        Vector3 targetPosition = targetObject.transform.position;
        Vector3 newCameraPosition = new Vector3(targetPosition.x - offsetX, originalPosition.y, transform.position.z);

        // 흔들림 적용 (Y 값이 원래 높이를 유지하도록 보정)
        if (cameraShake != null)
        {
            newCameraPosition += cameraShake.ShakeOffset;
            newCameraPosition.y = originalPosition.y; // Y 값 유지
        }

        transform.position = newCameraPosition;
    }
}
