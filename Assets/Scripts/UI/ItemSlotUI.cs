using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    Item myItem;
    [SerializeField] Image itemImage;

    public void Assign(Item item)
    {
        myItem = item;
        itemImage.gameObject.SetActive(item != null);
        if (myItem != null)
        {
            itemImage.sprite = item.Sprite;
        }
    }
}
