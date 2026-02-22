using UnityEngine;

public class UI_Inventory : MonoBehaviour
{
    private Inventory inventory;
    private UI_ItemSlot[] itemSlots;

    public void SetInventory(Inventory inventory){
        this.inventory = inventory;
        RefreshUI();
    }

    public void RefreshUI() {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (i < inventory.itemList.Count)
                itemSlots[i].UpdateSlot(inventory.itemList[i]);
            else
                itemSlots[i].UpdateSlot(null);
        }
    }


}
