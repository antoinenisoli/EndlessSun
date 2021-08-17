using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    Item myItem;
    [SerializeField] Image itemImage;
    Button myButton;

    private void Awake()
    {
        myButton = GetComponentInChildren<Button>();
    }

    public void Assign(Item item)
    {
        myItem = item;
        itemImage.gameObject.SetActive(item != null);
        if (myItem != null)
        {
            itemImage.sprite = item.Sprite;
        }
    }

    private void OnEnable()
    {
        itemImage.enabled = myItem != null;
        myButton.enabled = myItem != null;
    }

    public void ShowTooltip()
    {
        if (myItem == null)
            return;

        TooltipSystem.Instance.Show(myItem.Name, myItem.ToString(), itemImage.rectTransform);
    }

    public void HideTooltip()
    {
        if (myItem == null)
            return;

        TooltipSystem.Instance.Hide();
    }

    public void Select()
    {
        TooltipSystem.Instance.Hide();
        myItem.Effect();
        PlayerInventory.Instance.RemoveItem(myItem);
        myItem = null;
        OnEnable();
    }
}
