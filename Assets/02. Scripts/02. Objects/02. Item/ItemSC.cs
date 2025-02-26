using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSC : MonoBehaviour
{
    public Item data;
    private SpriteRenderer spriteRenderer;

    [Header("Trigger π¸¿ß")]
    public Vector2 triggerSize = new Vector2(1f, 1f);
    public float triggerOffset = 0f;

    private void Start()
    {
        Invoke(nameof(InitializeComponents), 0.001f);
        Invoke(nameof(SetupItem), 0.001f);
        Invoke(nameof(SetupTriggerCollider), 0.001f);
    }

    private void SetupItem()
    {
        if (data.icon != null)
            spriteRenderer.sprite = data.icon;
        else
            Debug.LogWarning("iconæ¯¿Ω");
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
            Debug.Log("PlayerStat æ¯¿Ω");
            return;
        }
        switch (data.Type)
        {
            case ItemType.Score:


               // ScoreManager.Instance.AddScore(data.score);
                Debug.Log($"?êÏàò?çÎìù :   {data.score}");

                ScoreManager.Instance.AddScore(data.score);
                Debug.Log($"?†ê?àò?öç?ìù :   {data.score}");


               // ScoreManager.Instance.AddScore(data.score);
                Debug.Log($"?†ê?àò?öç?ìù :   {data.score}");
                ScoreManager.Instance.AddScore(data.score);
                Debug.Log($"Ω∫ƒ⁄æÓ »πµÊ: {data.score}");

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
                if (GameManager.Instance.GetBonusItem(data.name))
                {
                    //?ù¥ÎØ? Î≥¥ÎÑà?ä§ ?ïÑ?ù¥?Öú?ùÑ ?öç?ìù?ïú Í≤ΩÏö∞ ?†ê?àò?öç?ìù
                    ScoreManager.Instance.AddScore(data.score);
                    Debug.Log($"Ω∫ƒ⁄æÓ »πµÊ: {data.score}");
                }
                else
                {
                    Debug.Log($"∫∏≥ Ω∫æ∆¿Ã≈€ »πµÊ : {data.name}");
                }

                if (GameManager.Instance.IsGetAllBonusItem())
                {
                    GameManager.Instance.ResetBonusItem();
                }
                break;
            case ItemType.PowerUPItem:
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
                Debug.Log($"∏”¥œæ∆¿Ã≈€ »πµÊ : {data.money}");
                break;
            default:
                Debug.Log("Default");
                break;
        }
        Destroy(this.gameObject);
    }
}

