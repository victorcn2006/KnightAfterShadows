using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    private Inventory inventory;
    [SerializeField] private UI_Inventory uiInventory;

    private void Awake() {
        Debug.Log("[Player] Initializing...");

        // Check if inventory was already loaded by login flow
        if (InventoryManager.instance != null)
        {
            inventory = InventoryManager.instance.GetCurrentInventory();

            if (inventory != null)
            {
                Debug.Log($"[Player] ? Inventory loaded from InventoryManager for user {inventory.userId}");
            }
            else
            {
                Debug.LogWarning("[Player] ? Inventory was null in InventoryManager");
                // Create new inventory if none exists
                int userId = UserSession.UserId;
                if (userId != -1)
                {
                    Debug.Log("[Player] Creating new inventory...");
                    inventory = new Inventory(userId);
                }
                else
                {
                    Debug.LogError("[Player] User not logged in!");
                    return;
                }
            }
        }
        else
        {
            Debug.LogError("[Player] ? InventoryManager instance not found!");
            return;
        }

        // Setup UI
        if (uiInventory != null)
        {
            uiInventory.SetInventory(inventory);
            Debug.Log("[Player] ? UI_Inventory set up successfully");
        }
        else
        {
            Debug.LogError("[Player] ? UI_Inventory not assigned in inspector!");
        }
    }

    private void OnDestroy() {
        // Save inventory when player is destroyed or scene is unloaded
        if (InventoryManager.instance != null && inventory != null)
        {
            InventoryManager.instance.SaveCurrentInventory();
            Debug.Log("[Player] ? Inventory saved on destroy");
        }
    }
}