using UnityEngine;

public class Item {
    public ItemData itemData;
    public int amount;

    // Constructor with ItemData
    public Item(ItemData data, int amount = 1) {
        this.itemData = data;
        this.amount = amount;
    }

    // Get item name
    public string GetName() {
        return itemData != null ? itemData.itemName : "Unknown Item";
    }

    // Get item icon from ItemData
    public Sprite GetIcon() {
        return itemData != null ? itemData.icon : null;
    }

    // Get item ID
    public int GetItemId() {
        return itemData != null ? itemData.id : -1;
    }

    // Get rarity color
    public Color GetRarityColor() {
        return itemData != null ? itemData.GetRarityColor() : Color.white;
    }

    // Check if stackable
    public bool IsStackable() {
        return itemData != null && itemData.stackable;
    }

    // Get max stack amount
    public int GetMaxStackAmount() {
        return itemData != null ? itemData.maxStackAmount : 1;
    }

    // Get item amount display
    public int GetAmount() {
        return amount;
    }

    public override string ToString() {
        return $"{GetName()} x{amount}";
    }
}