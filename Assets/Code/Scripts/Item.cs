

using UnityEngine;

public class Item
{
    public enum Item_Type {
        Sword = 0,
        Bow = 1,
        Crossbow = 2,
        HealthPotion = 3,
        SpeedPotion = 4,
        Arrows = 5
    }

    public Item_Type itemType;
    public int amount;

    // Constructor
    public Item(Item_Type type, int amount = 1) {
        this.itemType = type;
        this.amount = amount;
    }

    // Get item name
    public string GetName() {
        return itemType.ToString();
    }

    // Get item icon from resources
    public Sprite GetIcon() {
        return Resources.Load<Sprite>($"Items/{itemType.ToString()}");
    }

    // Get item amount display
    public int GetAmount() {
        return amount;
    }

    public override string ToString() {
        return $"{itemType} x{amount}";
    }
}
