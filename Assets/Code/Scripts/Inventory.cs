using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    public int userId;
    public List<Item> itemList;
    public int maxSlots = 8;

    /// <summary>
    /// Constructor for creating a new inventory
    /// </summary>
    public Inventory(int userId) {
        this.userId = userId;
        this.itemList = new List<Item>();
    }

    /// <summary>
    /// Constructor for loading from database
    /// </summary>
    public Inventory(int userId, List<Item> items, int maxSlots = 8) {
        this.userId = userId;
        this.itemList = items;
        this.maxSlots = maxSlots;
    }

    /// <summary>
    /// Add an item to the inventory with stacking support
    /// </summary>
    public void AddItem(Item item) {
        // Try to stack with existing items first
        bool stacked = false;
        foreach (var existingItem in itemList)
        {
            if (existingItem.itemType == item.itemType &&
                existingItem.amount < 99)
            { // Max stack of 99
                existingItem.amount += item.amount;
                stacked = true;
                Debug.Log($"[Inventory] Item stacked. New amount: {existingItem.amount}");
                break;
            }
        }

        // If not stacked and inventory has space, add as new item
        if (!stacked && itemList.Count < maxSlots)
        {
            itemList.Add(item);
            Debug.Log($"[Inventory] Item added: {item.itemType}");
        }
        else if (!stacked)
        {
            Debug.LogWarning("[Inventory] Inventory is full! Cannot add item.");
        }
    }

    /// <summary>
    /// Remove an item at a specific index
    /// </summary>
    public void RemoveItem(int index) {
        if (index >= 0 && index < itemList.Count)
        {
            Item removed = itemList[index];
            itemList.RemoveAt(index);
            Debug.Log($"[Inventory] Removed item at index {index}: {removed.itemType}");
        }
    }

    /// <summary>
    /// Check if inventory is full
    /// </summary>
    public bool IsFull() {
        return itemList.Count >= maxSlots;
    }

    /// <summary>
    /// Get the number of empty slots
    /// </summary>
    public int GetEmptySlots() {
        return maxSlots - itemList.Count;
    }
}
