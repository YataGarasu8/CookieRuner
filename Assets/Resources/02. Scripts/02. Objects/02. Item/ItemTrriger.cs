using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTrriger : MonoBehaviour
{
    private ItemSC Item;

    public void SetItem(ItemSC item)
    {
        Item = item;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ItemColider"))
        {
            Debug.Log("if¹® ³»ºÎ");
            Item.OnPlayerDetected(collision);
        }
    }
}
