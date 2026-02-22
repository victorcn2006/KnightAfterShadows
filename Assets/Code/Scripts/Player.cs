using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Inventory inventory;
    [SerializeField] private UI_Inventory uiInventory;

    private void Awake() {
        // Get the loaded inventory from InventoryManager
        if (InventoryManager.instance != null)
        {
            inventory = InventoryManager.instance.GetCurrentInventory();
            if (inventory != null)
            {
                uiInventory.SetInventory(inventory);
                Debug.Log("[Player] Inventory loaded and UI updated");
            }
            else
            {
                Debug.LogError("[Player] No inventory loaded for user!");
            }
        }
        else
        {
            Debug.LogError("[Player] InventoryManager instance not found!");
        }
    }

    private void OnDestroy() {
        // Save inventory when player is destroyed or scene is unloaded
        if (InventoryManager.instance != null)
        {
            InventoryManager.instance.SaveCurrentInventory();
        }
    }
}
