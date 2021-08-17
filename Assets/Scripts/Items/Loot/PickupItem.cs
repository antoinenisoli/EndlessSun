using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PickupItem : LootItem
{
    [Header("PICK UP ITEM")]
    [SerializeField] protected ItemProfile itemProfile;

    public override void Interact()
    {
        PlayerInventory.Instance.AddItem(itemProfile.Item);
        Destroy(gameObject);
    }

    public override string ToString()
    {
        return "Pick";
    }
}
