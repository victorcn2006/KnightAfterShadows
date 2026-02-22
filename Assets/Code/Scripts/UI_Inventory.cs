using UnityEngine;

public class UI_Inventory : MonoBehaviour
{
    private Inventory inventory;
    private UI_ItemSlot[] itemSlots;
    private UI_ItemSlot selectedSlot;

    private void Awake() {
        itemSlots = GetComponentsInChildren<UI_ItemSlot>();
    }

    public void SetInventory(Inventory inventory){
        this.inventory = inventory;
        RefreshUI();
    }

    public void RefreshUI() {
        if (inventory == null) return;

        // Update each slot with items from inventory
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (i < inventory.itemList.Count)
            {
                itemSlots[i].UpdateSlot(inventory.itemList[i]);
            }
            else
            {
                itemSlots[i].UpdateSlot(null);
            }
        }
    }

    public void OnSlotClicked(UI_ItemSlot clickedSlot) {
        // Deselect previous slot
        if (selectedSlot != null)
        {
            selectedSlot.SetSelected(false);
        }

        // Select new slot
        selectedSlot = clickedSlot;
        selectedSlot.SetSelected(true);

        Item selectedItem = selectedSlot.GetItem();
        if (selectedItem != null)
        {
            Debug.Log($"Selected: {selectedItem.GetName()} x{selectedItem.amount}");
        }
    }

    public Item GetSelectedItem() {
        return selectedSlot != null ? selectedSlot.GetItem() : null;
    }
}
