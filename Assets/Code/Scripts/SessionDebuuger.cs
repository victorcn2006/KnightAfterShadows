using UnityEngine;

public class SessionDebugger : MonoBehaviour {
    private void Update() {
        // Press D to show current session info
        if (Input.GetKeyDown(KeyCode.D))
        {
            PrintSessionInfo();
        }

        // Press C to clear session (simulate logout)
        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearSession();
        }
    }

    private void PrintSessionInfo() {
        Debug.Log("=== SESSION DEBUG INFO ===");

        // Check UserSession
        Debug.Log($"[UserSession] UserId: {UserSession.UserId}");
        Debug.Log($"[UserSession] Username: {UserSession.Username}");
        Debug.Log($"[UserSession] IsLoggedIn: {UserSession.IsLoggedIn}");

        // Check InventoryManager
        if (InventoryManager.instance != null)
        {
            var inventory = InventoryManager.instance.GetCurrentInventory();
            if (inventory != null)
            {
                Debug.Log($"[InventoryManager] Inventory exists: YES");
                Debug.Log($"[InventoryManager] Inventory userId: {inventory.userId}");
                Debug.Log($"[InventoryManager] Inventory slots: {inventory.itemList.Count}/{inventory.maxSlots}");
                Debug.Log($"[InventoryManager] Items in inventory: {inventory.itemList.Count}");
            }
            else
            {
                Debug.Log($"[InventoryManager] Inventory exists: NO (null)");
            }
        }
        else
        {
            Debug.Log("[InventoryManager] Instance not found!");
        }

        Debug.Log("========================");
    }

    private void ClearSession() {
        Debug.Log("[Debug] Clearing session...");
        UserSession.Logout();
        InventoryManager.instance.ClearInventory();
        Debug.Log("[Debug] Session cleared");
    }
}
