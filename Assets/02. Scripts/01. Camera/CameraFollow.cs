using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraFollow : MonoBehaviour
{
    public GameObject targetObject;

    [Header("카메라 오프셋 설정")]
    public float offsetX = 0f;
    public float offsetY = 0f;

    private Vector3 initialOffset;

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
        offsetY += initialOffset.y;
    }

    void Update()
    {
        if (targetObject == null) return;

        // 실시간으로 적용되는 오프셋 반영
        Vector3 targetPosition = targetObject.transform.position;
        Vector3 newCameraPosition = new Vector3(targetPosition.x - offsetX, targetPosition.y - offsetY, transform.position.z);
        transform.position = newCameraPosition;
    }
}
