using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSC : MonoBehaviour
{
    public Item data;
    private SpriteRenderer spriteRenderer;
    //private Animator animator;
    //private Rigidbody2D rb;

    [Header("Trigger ?¬ê¸°")]
    public Vector2 triggerSize = new Vector2(1f, 1f);
    public float triggerOffset = 0f;

    private void Start()
    {
        Invoke(nameof(InitializeComponents), 0.001f);
        Invoke(nameof(SetupItem), 0.001f);
        Invoke(nameof(SetupTriggerCollider), 0.001f);
        //InitializeComponents();
        //SetupItem();
        //SetupTriggerCollider();
    }

    private void SetupItem()
    {
        if (data.icon != null)
            spriteRenderer.sprite = data.icon;
        else
            Debug.LogWarning("?¤í”„?¼ì´?¸ê? ?†ìŒ.");
    }
    private void InitializeComponents()
    {
        // SpriteRenderer 
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        // Collider 
        var collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
        }
        collider.size = data.size;
        collider.isTrigger = true;

        // Rigidbody2D
        /*rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.gravityScale = 0;
        rb.bodyType = RigidbodyType2D.Static; // ï¿½ï¿½Ö¹ï¿½ï¿½ï¿?ï¿½ï¿½ï¿½ï¿½ (ï¿½ï¿½ï¿½ï¿½ï¿½Ì´ï¿½ ï¿½ï¿½Ö¹ï¿½ï¿½ï¿?ï¿½Úµï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½)*/
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
        PlayerStats playerStats = collider.GetComponentInParent<PlayerStats>();
        PlayerMovement playerMovement = collider.GetComponentInParent<PlayerMovement>();

        if (playerStats == null)
        {
            Debug.Log("PlayerStat?†ìŒ");
            return;
        }
        Debug.Log("Item Trigger?— Playerê°? ê°ì???¨!");
        switch (data.Type)
        {
            case ItemType.Score:

               // ScoreManager.Instance.AddScore(data.score);
                Debug.Log($"?ìˆ˜?ë“ :   {data.score}");

                ScoreManager.Instance.AddScore(data.score);
                Debug.Log($"? ?ˆ˜?š?“ :   {data.score}");

                break;
            case ItemType.HPUPItem:
                playerStats.Heal(data.healthBonus);
                break;
            case ItemType.SpeedUPItem:
                if (playerStats.isSpeedUP)
                {
                    playerStats.CancelInvoke(nameof(playerStats.ResetSpeedModifier));
                    playerStats.Invoke(nameof(playerStats.ResetSpeedModifier), data.duration);
                }
                else
                {
                    playerStats.ModifySpeed(data.speedBonus);
                    playerStats.Invoke(nameof(playerStats.ResetSpeedModifier), data.duration);
                }
                break;
            case ItemType.TreasureItem:
                playerStats.Heal(data.healthBonus);
                if (playerStats.isSpeedUP)
                {
                    playerStats.CancelInvoke(nameof(playerStats.ResetSpeedModifier));
                    playerStats.Invoke(nameof(playerStats.ResetSpeedModifier), data.duration);
                }
                else
                {
                    playerStats.ModifySpeed(data.speedBonus);
                    playerStats.Invoke(nameof(playerStats.ResetSpeedModifier), 5f);
                }
                break;
            case ItemType.BonusItem:
                if(GameManager.Instance.GetBonusItem(data.name))
                {
                    //?´ë¯? ë³´ë„ˆ?Š¤ ?•„?´?…œ?„ ?š?“?•œ ê²½ìš° ? ?ˆ˜?š?“
                    ScoreManager.Instance.AddScore(data.score);
                    Debug.Log($"? ?ˆ˜?š?“ :   {data.score}");
                }
                else
                {
                    Debug.Log($"ë³´ë„ˆ?Š¤ ?•„?´?…œ ?š?“ : {data.name}");
                }

                if (GameManager.Instance.IsGetAllBonusItem())
                {
                    //ë³´ë„ˆ?Š¤ ?Š¤?…Œ?´ì§?ë¡? ?´?™ ì²˜ë¦¬
                    GameManager.Instance.ResetBonusItem();
                }
                break;
            case ItemType.PowerUPItem:
                Debug.Log("?Œì›Œ?…ì•„?´í…œ ?ë“");
                if (playerMovement.isItemInvincible)
                {
                    playerMovement.CancelInvoke(nameof(playerMovement.TolggleImmune));
                    playerMovement.Invoke(nameof(playerMovement.TolggleImmune), data.duration);
                }
                else
                {
                    playerStats.IncreaseSize();
                    playerMovement.TolggleImmune();
                    playerMovement.Invoke(nameof(playerMovement.TolggleImmune), data.duration);
                }
                break;
            case ItemType.MoneyItem:
                Debug.Log($"???„ì´???ë“ : {data.money}");
                break;
            default:
                Debug.Log("?”í´??");
                break;
        }
        Destroy(this.gameObject);
    }
}
