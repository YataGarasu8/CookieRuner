using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject targetObject;
    private CameraShakeLerp cameraShake; // CameraShakeLerp ��ũ��Ʈ ����

    [Header("ī�޶� ������ ����")]
    public float offsetX = 0f;
    public float offsetY = 0f;

    private Vector3 initialOffset;
    private Vector3 originalPosition; // ���� ī�޶� ��ġ

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

        // CameraShakeLerp ���� ��������
        cameraShake = GetComponent<CameraShakeLerp>();

        // �ʱ� ī�޶� ��ġ ����
        originalPosition = transform.position;
    }

    void LateUpdate() // ��鸲 ���� �� ī�޶� ��ġ ����
    {
        if (targetObject == null) return;

        // �⺻������ �÷��̾ ���󰡴� ��ġ ���
        Vector3 targetPosition = targetObject.transform.position;
        Vector3 newCameraPosition = new Vector3(targetPosition.x - offsetX, originalPosition.y, transform.position.z);

        // ��鸲 ���� (Y ���� ���� ���̸� �����ϵ��� ����)
        if (cameraShake != null)
        {
            newCameraPosition += cameraShake.ShakeOffset;
            newCameraPosition.y = originalPosition.y; // Y �� ����
        }

        transform.position = newCameraPosition;
    }
}
