using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObstacle : MonoBehaviour
{
    // ���� �ӵ�
    public float fallSpeed = 10f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("FallingObstacle ��ũ��Ʈ�� ������ ������Ʈ�� Rigidbody2D ������Ʈ�� �����ϴ�.");
            return;
        }

        // �ʱ� ���¸� Static���� �����Ͽ� �������� �ʵ��� ��
        rb.bodyType = RigidbodyType2D.Static;
    }

    // �÷��̾���� Trigger �浹 ����
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //Debug.Log("�÷��̾�� �浹��. ��ü�� Dynamic���� ��ȯ�ϰ� ���ϸ� �����մϴ�.");
            // Rigidbody2D�� BodyType�� Dynamic���� �����Ͽ� ���� ȿ���� �����ϰ� ��� ����
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.velocity = new Vector2(0f, -fallSpeed);
        }
    }
}
