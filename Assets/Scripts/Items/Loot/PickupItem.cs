using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PickupItem : LootItem
{
    [Header("PICK UP ITEM")]
    [SerializeField] protected ItemProfile itemProfile;

    public void ProposeToPick()
    {
        UIManager.Instance.ShowPickUp(this);
    }

    public void Pick()
    {
        PlayerInventory.Instance.AddItem(itemProfile.Item);
        Destroy(gameObject);
    }
}
