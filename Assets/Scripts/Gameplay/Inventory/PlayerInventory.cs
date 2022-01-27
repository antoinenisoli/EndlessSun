using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;
    [SerializeField] List<Item> inventoryItems = new List<Item>();

    public List<Item> InventoryItems { get => inventoryItems; set => inventoryItems = value; }

    private void Awake()
    {
        Singleton();
    }

    void Singleton()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public Item GetItem(int i)
    {
        if (i < 0 || i > InventoryItems.Count - 1)
            return null;
        else
            return InventoryItems[i];
    }

    public void AddItem(Item item)
    {
        InventoryItems.Add(item);
    }

    public void RemoveItem(Item item)
    {
        if (InventoryItems.Contains(item))
            InventoryItems.Remove(item);
    }
}
