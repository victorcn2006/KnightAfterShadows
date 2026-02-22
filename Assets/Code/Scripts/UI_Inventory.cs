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

    /// <summary>
    /// Drop 1 item from the selected stack
    /// </summary>
    public void DropSelectedItem() {
        Item selectedItem = GetSelectedItem();
        if (selectedItem == null)
        {
            Debug.LogWarning("[UI_Inventory] No item selected to drop!");
            return;
        }

        // Decrease amount
        selectedItem.amount--;
        Debug.Log($"[UI_Inventory] Dropped 1x {selectedItem.GetName()} (remaining: {selectedItem.amount})");

        // If stack is now empty, remove completely
        if (selectedItem.amount <= 0)
        {
            RemoveSelectedItemFromInventory();
            Debug.Log($"[UI_Inventory] Stack empty, removed item completely");
        }

        // Save and refresh
        InventoryManager.instance.SaveCurrentInventory();
        RefreshUI();
    }

    /// <summary>
    /// Helper method to remove the selected item from inventory
    /// </summary>
    private void RemoveSelectedItemFromInventory() {
        if (selectedSlot == null) return;

        Item selectedItem = selectedSlot.GetItem();

        // Find and remove from inventory list
        for (int i = 0; i < inventory.itemList.Count; i++)
        {
            if (inventory.itemList[i] == selectedItem)
            {
                inventory.RemoveItem(i);
                break;
            }
        }

        // Deselect the slot
        selectedSlot.SetSelected(false);
        selectedSlot = null;
    }
}
