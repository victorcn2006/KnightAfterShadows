using UnityEngine;

public class InventoryTestHelper : MonoBehaviour {
    private void Update() {
        // Test adding different items with keyboard shortcuts
        if (Input.GetKeyDown(KeyCode.E))
        {
            AddTestItem(Item.Item_Type.Sword, 1);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            AddTestItem(Item.Item_Type.Bow, 1);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            AddTestItem(Item.Item_Type.Crossbow, 1);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            AddTestItem(Item.Item_Type.HealthPotion, 3);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            AddTestItem(Item.Item_Type.SpeedPotion, 2);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            AddTestItem(Item.Item_Type.Arrows, 5);
        }

        // Save inventory
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (InventoryManager.instance != null)
            {
                InventoryManager.instance.SaveCurrentInventory();
                Debug.Log("[Test] Inventory saved manually");
            }
        }

        // Refresh UI
        if (Input.GetKeyDown(KeyCode.R))
        {
            var uiInventory = FindObjectOfType<UI_Inventory>();
            if (uiInventory != null)
            {
                uiInventory.RefreshUI();
                Debug.Log("[Test] UI refreshed");
            }
        }
    }

    private void AddTestItem(Item.Item_Type itemType, int amount) {
        var inventory = InventoryManager.instance.GetCurrentInventory();
        if (inventory != null)
        {
            inventory.AddItem(new Item(itemType, amount));

            // Refresh UI
            var uiInventory = FindObjectOfType<UI_Inventory>();
            if (uiInventory != null)
            {
                uiInventory.RefreshUI();
            }

            Debug.Log($"[Test] Added {amount}x {itemType}");
        }
        else
        {
            Debug.LogError("[Test] No inventory loaded!");
        }
    }
}