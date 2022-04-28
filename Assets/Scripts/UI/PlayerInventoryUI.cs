using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryUI : HUD
{
    [SerializeField] Transform inventoryContent;
    ItemSlotUI[] itemSlots;

    private void Awake()
    {
        itemSlots = inventoryContent.GetComponentsInChildren<ItemSlotUI>();
        Close();
    }

    public override void Open()
    {
        base.Open();
        inventoryContent.gameObject.SetActive(true);
        inventoryContent.DOKill(true);
        UpdateUI();
        inventoryContent.DOScale(Vector3.one, 0.5f).SetEase(Ease.InCubic);
    }

    public override void Close()
    {
        base.Close();
        inventoryContent.DOKill(true);
        inventoryContent.DOScale(Vector3.one * 0.001f, 0.5f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            inventoryContent.gameObject.SetActive(false);
        });
    }

    public override void UpdateUI()
    {
        if (PlayerInventory.Instance)
        {
            for (int i = 0; i < itemSlots.Length; i++)
            {
                Item item = PlayerInventory.Instance.GetItem(i);
                if (item != null)
                    itemSlots[i].Assign(item);
                else
                    itemSlots[i].Assign(null);
            }
        }
    }
}
