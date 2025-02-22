using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraFollow : MonoBehaviour
{
    public GameObject targetObject;

    [Header("ī�޶� ������ ����")]
    public float offsetX = 0f;
    public float offsetY = 0f;

    private Vector3 initialOffset;

    void Start()
    {
        if (targetObject == null)
        {
            Debug.LogError("Ÿ�� ������Ʈ�� �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }

        // �ʱ� ������ ���
        initialOffset = targetObject.transform.position - transform.position;
        offsetX += initialOffset.x;
        offsetY += initialOffset.y;
    }

    void Update()
    {
        if (targetObject == null) return;

        // �ǽð����� ����Ǵ� ������ �ݿ�
        Vector3 targetPosition = targetObject.transform.position;
        Vector3 newCameraPosition = new Vector3(targetPosition.x - offsetX, targetPosition.y - offsetY, transform.position.z);
        transform.position = newCameraPosition;
    }
}
