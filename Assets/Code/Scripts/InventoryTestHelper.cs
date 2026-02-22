using UnityEngine;

public class InventoryTestHelper : MonoBehaviour {

    [Header("ItemData Assets - Drag Here!")]
    [SerializeField] private ItemData swordData;
    [SerializeField] private ItemData bowData;
    [SerializeField] private ItemData crossbowData;
    [SerializeField] private ItemData healthPotionData;
    [SerializeField] private ItemData speedPotionData;
    [SerializeField] private ItemData arrowsData;

    private UI_Inventory uiInventory;
    private Inventory inventory;

    private void Start() {
        Debug.Log("[InventoryTestHelper] Starting...");

        // Cache the UI_Inventory reference
        uiInventory = FindObjectOfType<UI_Inventory>();
        if (uiInventory == null)
        {
            Debug.LogError("[InventoryTestHelper] ❌ UI_Inventory not found in scene!");
            return;
        }
        Debug.Log("[InventoryTestHelper] ✓ UI_Inventory found");

        // Cache the inventory reference
        if (InventoryManager.instance != null)
        {
            inventory = InventoryManager.instance.GetCurrentInventory();
            if (inventory == null)
            {
                Debug.LogError("[InventoryTestHelper] ❌ Inventory is null! Was LoadInventoryForUser called?");
                return;
            }
            Debug.Log($"[InventoryTestHelper] ✓ Inventory loaded for user {inventory.userId}. Items: {inventory.itemList.Count}");
        }
        else
        {
            Debug.LogError("[InventoryTestHelper] ❌ InventoryManager.instance is null!");
            return;
        }

        // Check ItemData assignments
        Debug.Log("[InventoryTestHelper] Checking ItemData assignments...");
        if (swordData == null) Debug.LogWarning("[InventoryTestHelper] ⚠ Sword ItemData not assigned in inspector!");
        if (bowData == null) Debug.LogWarning("[InventoryTestHelper] ⚠ Bow ItemData not assigned in inspector!");
        if (crossbowData == null) Debug.LogWarning("[InventoryTestHelper] ⚠ Crossbow ItemData not assigned in inspector!");
        if (healthPotionData == null) Debug.LogWarning("[InventoryTestHelper] ⚠ Health Potion ItemData not assigned in inspector!");
        if (speedPotionData == null) Debug.LogWarning("[InventoryTestHelper] ⚠ Speed Potion ItemData not assigned in inspector!");
        if (arrowsData == null) Debug.LogWarning("[InventoryTestHelper] ⚠ Arrows ItemData not assigned in inspector!");
    }

    private void Update() {
        // Verify everything is still available
        if (inventory == null || uiInventory == null)
        {
            return;
        }

        // Test adding different items with keyboard shortcuts
        if (Input.GetKeyDown(KeyCode.E) && swordData != null)
        {
            AddTestItem(swordData, 1);
        }
        if (Input.GetKeyDown(KeyCode.B) && bowData != null)
        {
            AddTestItem(bowData, 1);
        }
        if (Input.GetKeyDown(KeyCode.C) && crossbowData != null)
        {
            AddTestItem(crossbowData, 1);
        }
        if (Input.GetKeyDown(KeyCode.H) && healthPotionData != null)
        {
            AddTestItem(healthPotionData, 3);
        }
        if (Input.GetKeyDown(KeyCode.J) && speedPotionData != null)
        {
            AddTestItem(speedPotionData, 2);
        }
        if (Input.GetKeyDown(KeyCode.A) && arrowsData != null)
        {
            AddTestItem(arrowsData, 5);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            uiInventory.DropSelectedItem();
        }

        // Save inventory
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (InventoryManager.instance != null)
            {
                InventoryManager.instance.SaveCurrentInventory();
                Debug.Log("[Test] ✓ Inventory saved to database");
            }
        }

        // Refresh UI
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (uiInventory != null)
            {
                uiInventory.RefreshUI();
                Debug.Log("[Test] ✓ UI refreshed manually");
            }
        }

        // Show inventory status
        if (Input.GetKeyDown(KeyCode.I))
        {
            ShowInventoryStatus();
        }
    }

    private void AddTestItem(ItemData itemData, int amount) {
        if (inventory == null)
        {
            Debug.LogError("[Test] ❌ Inventory is null!");
            return;
        }

        if (itemData == null)
        {
            Debug.LogError("[Test] ❌ ItemData is null!");
            return;
        }

        if (uiInventory == null)
        {
            Debug.LogError("[Test] ❌ UI_Inventory is null!");
            return;
        }

        // Add item to inventory
        inventory.AddItem(new Item(itemData, amount));
        Debug.Log($"[Test] ✓ Added {amount}x {itemData.itemName}");

        // Refresh UI immediately
        uiInventory.RefreshUI();
        Debug.Log($"[Test] ✓ UI refreshed. Total items in inventory: {inventory.itemList.Count}");
    }

    private void ShowInventoryStatus() {
        if (inventory == null)
        {
            Debug.LogError("[Test] ❌ Inventory is null!");
            return;
        }

        Debug.Log("=== INVENTORY STATUS ===");
        Debug.Log($"  User ID: {inventory.userId}");
        Debug.Log($"  Total Items: {inventory.itemList.Count}/{inventory.maxSlots}");
        Debug.Log($"  Empty Slots: {inventory.GetEmptySlots()}");

        for (int i = 0; i < inventory.itemList.Count; i++)
        {
            var item = inventory.itemList[i];
            Debug.Log($"  Slot {i}: {item.GetName()} x{item.amount}");
        }
        Debug.Log("========================");
    }

}