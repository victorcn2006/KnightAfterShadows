using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Item> itemList;

    public Inventory() {
        itemList = new List<Item>();
        AddItem(new Item { itemType = Item.Item_Type.Sword, amount = 1 });
        Debug.Log(itemList.Count);
        Debug.Log("Inventory");
    }

    public void AddItem(Item item) {
        itemList.Add(item);
    }
}
