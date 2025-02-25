using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSC : MonoBehaviour
{

    public Item data;
    private SpriteRenderer spriteRenderer;
    //private Animator animator;
    //private Rigidbody2D rb;

    [Header("Trigger 감지 설정")]
    public Vector2 triggerSize = new Vector2(1f, 1f);
    public float triggerOffset = 0f;

    private void Start()
    {
        Invoke(nameof(InitializeComponents), 0.001f);
        Invoke(nameof(SetupItem), 0.001f);
        Invoke(nameof(SetupTriggerCollider), 0.001f);
        //InitializeComponents();
        //SetupItem();
        //SetupTriggerCollider(); // 트리거 콜라이더 추가
    }

    private void SetupItem()
    {
        if (data.icon != null)
            spriteRenderer.sprite = data.icon;
        else
            Debug.LogWarning("아이템에 스프라이트가 지정되지 않았습니다.");
    }
    private void InitializeComponents()
    {
        // SpriteRenderer 설정
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        // Collider 설정
        var collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
        }
        collider.size = data.size;
        collider.isTrigger = true;

        // Rigidbody2D 설정
        /*rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 0;
        rb.bodyType = RigidbodyType2D.Static; // 장애물은 고정 (움직이는 장애물은 코드 내에서 변경)*/
    }

    private void SetupTriggerCollider()
    {
        GameObject triggerArea = new GameObject("TriggerArea");
        triggerArea.transform.parent = transform;
        triggerArea.transform.localPosition = new Vector3(-triggerOffset, 0, 0);

        BoxCollider2D triggerCollider = triggerArea.AddComponent<BoxCollider2D>();
        triggerCollider.isTrigger = true;
        triggerCollider.size = triggerSize;

        ItemTrriger trriger = triggerArea.AddComponent<ItemTrriger>();
        trriger.SetItem(this);
    }

    public void OnPlayerDetected(Collider2D collider)
    {
        PlayerStats playerStats = collider.GetComponent<PlayerStats>();

        Vector2 coli = new Vector2(collider.gameObject.transform.position.x, collider.gameObject.transform.position.y);
        Vector2 item = new Vector2((float)transform.position.x, (float)this.transform.position.y);
        if (playerStats == null)
        {
            Debug.Log("컴포넌트 없음");
            return;
        }
        Debug.Log("Trigger 영역에서 플레이어 감지!");
        // 감지 시 추가 로직 가능 (예: 팝업 시작, 공격 준비 등)
        switch (data.Type)
        {
            case ItemType.Score:
                ScoreManager.Instance.AddScore(data.score);
                Debug.Log($"스코어 획득 :   {data.score}");
                break;
            case ItemType.HPUPItem:
                playerStats.Heal(data.healthBonus);
                break;
            case ItemType.SpeedUPItem:
                if (playerStats.isSpeedUP)
                {
                    playerStats.CancelInvoke(nameof(playerStats.ResetSpeedModifier));
                    playerStats.Invoke(nameof(playerStats.ResetSpeedModifier), 5f);
                }
                else
                {
                    playerStats.ModifySpeed(data.speedBonus);
                    playerStats.Invoke(nameof(playerStats.ResetSpeedModifier), 5f);
                }
                break;
            case ItemType.TreasureItem:
                playerStats.Heal(data.healthBonus);
                if (playerStats.isSpeedUP)
                {
                    playerStats.CancelInvoke(nameof(playerStats.ResetSpeedModifier));
                    playerStats.Invoke(nameof(playerStats.ResetSpeedModifier), 5f);
                }
                else
                {
                    playerStats.ModifySpeed(data.speedBonus);
                    playerStats.Invoke(nameof(playerStats.ResetSpeedModifier), 5f);
                }
                break;
            case ItemType.BonusItem:
                //보너스아이템 처리
                break;
            case ItemType.PowerUPItem:
                playerStats.IncreaseSize();

                //무적처리
                break;
            default:
                Debug.Log("디폴트");
                break;
        }
        Destroy(this);
    }
}
