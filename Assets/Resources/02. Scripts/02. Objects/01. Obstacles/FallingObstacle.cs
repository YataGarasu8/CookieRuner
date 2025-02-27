using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObstacle : MonoBehaviour
{
    // 낙하 속도
    public float fallSpeed = 10f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("FallingObstacle 스크립트가 부착된 오브젝트에 Rigidbody2D 컴포넌트가 없습니다.");
            return;
        }

        // 초기 상태를 Static으로 설정하여 움직이지 않도록 함
        rb.bodyType = RigidbodyType2D.Static;
    }

    // 플레이어와의 Trigger 충돌 감지
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //Debug.Log("플레이어와 충돌함. 강체를 Dynamic으로 전환하고 낙하를 시작합니다.");
            // Rigidbody2D의 BodyType을 Dynamic으로 변경하여 물리 효과를 적용하고 즉시 낙하
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.velocity = new Vector2(0f, -fallSpeed);
        }
    }
}
