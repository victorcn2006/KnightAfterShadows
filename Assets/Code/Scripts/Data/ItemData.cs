using UnityEngine;

[CreateAssetMenu(fileName = "Item_", menuName = "Items/Create Item", order = 1)]
public class ItemData : ScriptableObject {
    public int id;
    public string itemName;
    [TextArea(3, 5)]
    public string description;
    public Sprite icon;
    public bool stackable = true;
    public int maxStackAmount = 99;
    public int rarity; // 0=Common, 1=Uncommon, 2=Rare, 3=Epic, 4=Legendary

    public enum Rarity {
        Common = 0,
        Uncommon = 1,
        Rare = 2,
        Epic = 3,
        Legendary = 4
    }

    public Color GetRarityColor() {
        return rarity switch
        {
            0 => Color.white,           // Common - White
            1 => Color.green,           // Uncommon - Green
            2 => Color.blue,            // Rare - Blue
            3 => new Color(1f, 0.5f, 0f), // Epic - Orange
            4 => Color.yellow,          // Legendary - Yellow
            _ => Color.white
        };
    }

    public string GetRarityName() {
        return ((Rarity)rarity).ToString();
    }
}