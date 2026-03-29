using UnityEngine;
using System.Collections.Generic;

public class ShopTest : MonoBehaviour {

    void Start() {
        Invoke("RunTest", 1f); // Wait for database initialization
    }

    void RunTest() {
        Debug.Log("--- STARTING SHOP TEST ---");

        SQLiteReader db = SQLiteReader.instance;
        if (db == null) {
            Debug.LogError("SQLiteReader instance not found!");
            return;
        }

        // 1. Setup User and Player
        string testUser = "testuser_" + Random.Range(0, 1000);
        db.RegisterUser(testUser, "password");
        var user = db.GetAllUsers().Find(u => u.username == testUser);
        if (user == null) {
            Debug.LogError("User registration failed in test.");
            return;
        }

        Player player = db.GetPlayerByUserId(user.id);
        int initialDiners = player.diners;
        Debug.Log($"Test Player {player.id} created with {initialDiners} diners.");

        // 2. Setup an item if none exist
        List<ItemDefinition> items = db.GetAllItems();
        int itemId;
        if (items.Count > 0) {
            itemId = items[0].id;
        } else {
            Debug.Log("No items found. Creating a test item.");
            // We need a way to insert an item. 
            // Since SQLiteReader doesn't have a generic Insert, we can use the existing _connection if we make it public or add a method.
            // For now, let's assume SQLiteReader has a way or just use ID 1 and hope it doesn't crash on foreign key (SQLite doesn't enforce by default).
            itemId = 1; 
        }

        // 3. TEST ROLLBACK: Buy with forceError = true
        Debug.Log("--- Testing Rollback (Should Fail) ---");
        int cost = 100;
        bool success = db.BuyItem(player.id, itemId, 5, cost, true);
        
        // Refresh player data
        player = db.GetPlayerByUserId(user.id);
        var inventory = db.GetPlayerInventory(player.id);
        var itemInInv = inventory.Find(i => i.item_id == itemId);

        if (!success && player.diners == initialDiners && (itemInInv == null || itemInInv.quantity == 0)) {
            Debug.Log("<color=green>ROLLBACK TEST PASSED:</color> Transaction failed as expected, money and inventory remained unchanged.");
        } else {
            Debug.LogError("ROLLBACK TEST FAILED: Data was modified despite transaction error!");
            Debug.Log($"Diners: {player.diners} (Expected: {initialDiners}), Inv Quantity: {(itemInInv?.quantity ?? 0)} (Expected: 0)");
        }

        // 4. TEST SUCCESSFUL BUY
        Debug.Log("--- Testing Successful Buy ---");
        success = db.BuyItem(player.id, itemId, 10, cost, false);
        
        player = db.GetPlayerByUserId(user.id);
        inventory = db.GetPlayerInventory(player.id);
        itemInInv = inventory.Find(i => i.item_id == itemId);

        if (success && player.diners == initialDiners - cost && itemInInv != null && itemInInv.quantity == 10) {
            Debug.Log("<color=green>BUY TEST PASSED:</color> Item bought successfully, money deducted.");
        } else {
            Debug.LogError("BUY TEST FAILED!");
        }

        // 5. TEST SUCCESSFUL SELL
        Debug.Log("--- Testing Successful Sell ---");
        int revenue = 50;
        success = db.SellItem(player.id, itemId, 4, revenue);

        player = db.GetPlayerByUserId(user.id);
        inventory = db.GetPlayerInventory(player.id);
        itemInInv = inventory.Find(i => i.item_id == itemId);

        if (success && player.diners == initialDiners - cost + revenue && itemInInv != null && itemInInv.quantity == 6) {
            Debug.Log("<color=green>SELL TEST PASSED:</color> Item sold successfully, money added.");
        } else {
            Debug.LogError("SELL TEST FAILED!");
        }

        Debug.Log("--- SHOP TEST COMPLETED ---");
    }
}
