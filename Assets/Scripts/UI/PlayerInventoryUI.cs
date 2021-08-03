using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryUI : HUD
{
    GridLayoutGroup gridGroup;

    private void Awake()
    {
        gridGroup = GetComponentInChildren<GridLayoutGroup>();
    }

    public override void UpdateUI()
    {
        for (int i = 0; i < gridGroup.transform.childCount; i++)
        {
            Transform itemSlot = gridGroup.transform.GetChild(i);
            Item item = PlayerInventory.Instance.GetItem(i);
            if (item != null)
            {
                itemSlot.GetChild(0).GetComponentInChildren<Image>().sprite = item.Sprite;
            }
            else
            {
                itemSlot.GetChild(0).GetComponentInChildren<Image>().sprite = null;
            }
        }
    }
}
