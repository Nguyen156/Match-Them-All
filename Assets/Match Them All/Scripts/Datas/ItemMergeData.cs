using System.Collections.Generic;
using UnityEngine;

public struct ItemMergeData
{
    public EItemName itemName;
    public List<Item> items;

    public ItemMergeData(Item firstItem)
    {
        itemName = firstItem.ItemName;

        items = new List<Item>();
        Add(firstItem);
    }

    public void Add(Item item) => items.Add(item);

    public void Remove(Item item) => items.Remove(item);

    public bool CanMergeItems() => items.Count >= 3;
}