using UnityEngine;

public class InventoryManager : MonoBehaviour {
    public static InventoryManager instance { get; private set; }
    private Inventory currentInventory;

    private void Awake() {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(this.gameObject);
    }

    /// <summary>
    /// Load the inventory for a specific user from the database
    /// </summary>
    public void LoadInventoryForUser(int userId) {
        currentInventory = SQLiteReader.instance.LoadInventory(userId);
        if (currentInventory == null)
        {
            // Create new inventory if none exists
            currentInventory = new Inventory(userId);
            SQLiteReader.instance.SaveInventory(currentInventory);
        }
        Debug.Log($"[InventoryManager] Loaded inventory for user {userId}");
    }

    /// <summary>
    /// Get the current active inventory
    /// </summary>
    public Inventory GetCurrentInventory() {
        return currentInventory;
    }

    /// <summary>
    /// Save the current inventory to the database
    /// </summary>
    public void SaveCurrentInventory() {
        if (currentInventory != null)
        {
            SQLiteReader.instance.SaveInventory(currentInventory);
            Debug.Log("[InventoryManager] Inventory saved");
        }
    }

    /// <summary>
    /// Clear the current inventory (use on logout)
    /// </summary>
    public void ClearInventory() {
        currentInventory = null;
    }

    private void OnDestroy() {
        SaveCurrentInventory();
    }
}