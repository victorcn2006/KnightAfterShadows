

public class Item
{
    public enum Item_Type { 
        Sword,
        Bow,
        HealthPotion,
        SpeedPotion,
        Arrows
    }

    public Item_Type itemType;
    public int amount;
}
