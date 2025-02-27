using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSC : MonoBehaviour
{
    public Item data;
    private SpriteRenderer spriteRenderer;

    [Header("Trigger ¹üÀ§")]
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
            Debug.LogWarning("icon¾øÀ½");
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

        if (playerStats == null || playerMovement == null)
        {
            Debug.Log("PlayerStat or playerMovement ¾øÀ½");
            return;
        }
        switch (data.Type)
        {
            case ItemType.Score:
                ScoreManager.Instance.AddScore(data.score);
                SoundManager.Instance.PlaySFX("JellySFX01");
                Debug.Log($"½ºÄÚ¾î È¹µæ: {data.score}");

                break;
            case ItemType.HPUPItem:
                playerStats.Heal(data.healthBonus);
                break;
            case ItemType.SpeedUPItem:
                if (playerStats.isSpeedUP)
                {
                    playerStats.CancelInvoke(nameof(playerStats.ResetSpeedModifier));
                    playerMovement.CancelInvoke(nameof(playerMovement.OffSpeedUPImmune));
                    playerStats.Invoke(nameof(playerStats.ResetSpeedModifier), data.duration);
                    playerMovement.Invoke(nameof(playerMovement.OffSpeedUPImmune), data.duration);
                }
                else
                {
                    playerStats.ModifySpeed(data.speedBonus);
                    playerMovement.OnSpeedUPImmune();
                    playerStats.Invoke(nameof(playerStats.ResetSpeedModifier), data.duration);
                    playerMovement.Invoke(nameof(playerMovement.OffSpeedUPImmune), data.duration);
                }
                break;
            case ItemType.TreasureItem:
                playerStats.Heal(data.healthBonus);
                if (playerStats.isSpeedUP)
                {
                    playerStats.CancelInvoke(nameof(playerStats.ResetSpeedModifier));
                    playerMovement.CancelInvoke(nameof(playerMovement.OffSpeedUPImmune));
                    playerStats.Invoke(nameof(playerStats.ResetSpeedModifier), data.duration);
                    playerMovement.Invoke(nameof(playerMovement.OffSpeedUPImmune), data.duration);
                }
                else
                {
                    playerStats.ModifySpeed(data.speedBonus);
                    playerMovement.OnSpeedUPImmune();
                    playerStats.Invoke(nameof(playerStats.ResetSpeedModifier), data.duration);
                    playerMovement.Invoke(nameof(playerMovement.OffSpeedUPImmune), data.duration);
                }
                break;
            case ItemType.BonusItem:
                if (GameManager.Instance.GetBonusItem(data.name))
                {
                    ScoreManager.Instance.AddScore(data.score);
                    Debug.Log($"½ºÄÚ¾î È¹µæ: {data.score}");
                }
                else
                {
                    Debug.Log($"º¸³Ê½º¾ÆÀÌÅÛ È¹µæ : {data.name}");
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
                    SoundManager.Instance.PlaySFX("PowerUPSFX01");
                    playerStats.IncreaseSize();
                    playerMovement.TolggleImmune();
                    playerMovement.Invoke(nameof(playerMovement.TolggleImmune), data.duration);
                }
                break;
            case ItemType.MoneyItem:
                GameManager.Instance.Money += data.money;
                Debug.Log($"¸Ó´Ï¾ÆÀÌÅÛ È¹µæ : {data.money}");
                Debug.Log($"ÃÑÇÕ :  {GameManager.Instance.Money}");
                SoundManager.Instance.PlaySFX("CoinSFX01");
                break;
            default:
                Debug.Log("Default");
                break;
        }
        Destroy(this.gameObject);
    }

    private void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }
}

