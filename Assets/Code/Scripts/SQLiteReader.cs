using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class SQLiteReader : MonoBehaviour {

    public static SQLiteReader instance { get; private set; }
    string dbPath => "URI=file:" + Application.dataPath + "/MyDatabase.sqlite";

    // Cache of all ItemData assets to avoid repeated Resources.LoadAll
    private static Dictionary<int, ItemData> itemDataCache = new Dictionary<int, ItemData>();
    private static bool cacheInitialized = false;

    #region Initialization

    private void Awake() {
        if (instance == null) instance = this;
        else Destroy(this.gameObject);

        // Initialize ItemData cache
        if (!cacheInitialized)
        {
            InitializeItemDataCache();
            cacheInitialized = true;
        }

        try
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                CreateItemTable(connection);
                CreatePlayer(connection);
                CreateInventory(connection);
                CreateInventoryItems(connection);
                CreateUsers(connection);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[SQLite] Initialization failed: {ex.Message}");
        }
    }

    private void InitializeItemDataCache() {
        Debug.Log("[SQLite] Initializing ItemData cache...");

        // Load all ItemData from Resources/Code/ScriptableObjects
        var allItems = Resources.LoadAll<ItemData>("Code/ScriptableObjects");

        if (allItems.Length == 0)
        {
            Debug.LogError("[SQLite] ❌ No ItemData found in Resources/Code/ScriptableObjects!");
            return;
        }

        Debug.Log($"[SQLite] Found {allItems.Length} ItemData assets");

        foreach (var itemData in allItems)
        {
            // If item doesn't have a valid ID, skip it
            if (itemData.id <= 0)
            {
                Debug.LogWarning($"[SQLite] ⚠ ItemData '{itemData.itemName}' has invalid ID: {itemData.id}. Skipping!");
                continue;
            }

            itemDataCache[itemData.id] = itemData;
            Debug.Log($"[SQLite] ✓ Cached: ID {itemData.id} → {itemData.itemName}");
        }

        Debug.Log($"[SQLite] ✓ ItemData cache initialized with {itemDataCache.Count} items");
    }

    private IDbConnection GetConnection() {
        return new SqliteConnection(dbPath);
    }

    #endregion

    #region Reader
    public List<ItemDefinition> GetAllItems() {
        var items = new List<ItemDefinition>();

        try
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM ITEM_DEFINITION;";

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new ItemDefinition
                            {
                                Id = reader.GetInt32(0),
                                Stackable = reader.GetInt32(1) == 1,
                                MaxAmount = reader.GetInt32(2),
                                Name = reader.GetString(3),
                                Description = reader.GetString(4)
                            });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[SQLite] Read failed: {ex.Message}");
        }

        return items;
    }

    #endregion

    #region Table Creation
    private void CreateItemTable(IDbConnection connection) {
        using (IDbCommand command = connection.CreateCommand())
        {
            command.CommandText = @"CREATE TABLE IF NOT EXISTS ITEM_DEFINITION (
            id INTEGER PRIMARY KEY AUTOINCREMENT, 
            stackable INTEGER,
            max_amount INTEGER, 
            name TEXT, 
            description TEXT)";

            command.ExecuteNonQuery();
        }
    }

    private void CreatePlayer(IDbConnection connection) {
        using (IDbCommand command = connection.CreateCommand())
        {
            command.CommandText =
            @"CREATE TABLE IF NOT EXISTS PLAYER(
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT,
                description TEXT,
                element text not null default 'Fire'
            )";

            command.ExecuteNonQuery();
        }
    }

    private void CreateInventory(IDbConnection connection) {
        using (IDbCommand command = connection.CreateCommand())
        {
            command.CommandText =
            @"CREATE TABLE IF NOT EXISTS INVENTORY(
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            user_id INTEGER NOT NULL UNIQUE,
            max_slots INTEGER DEFAULT 8,
            FOREIGN KEY(user_id) REFERENCES USERS(id))";

            command.ExecuteNonQuery();
        }
    }

    private void CreateInventoryItems(IDbConnection connection) {
        using (IDbCommand command = connection.CreateCommand())
        {
            command.CommandText =
            @"CREATE TABLE IF NOT EXISTS INVENTORY_ITEM(
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        item_definition_id INTEGER NOT NULL,
        inventory_id INTEGER NOT NULL,
        slot_index INTEGER,
        amount INTEGER,
        FOREIGN KEY(item_definition_id) REFERENCES ITEM_DEFINITION(id),
        FOREIGN KEY(inventory_id) REFERENCES INVENTORY(id))";

            command.ExecuteNonQuery();
        }
    }

    #endregion

    private void CreateUsers(IDbConnection connection) {
        using (IDbCommand command = connection.CreateCommand())
        {
            command.CommandText =
            @"CREATE TABLE IF NOT EXISTS USERS(
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            username TEXT UNIQUE NOT NULL,
            password_hash TEXT NOT NULL)";

            command.ExecuteNonQuery();
        }
    }

    public class ItemDefinition {
        public int Id;
        public bool Stackable;
        public int MaxAmount;
        public string Name;
        public string Description;
    }

    public int ValidateUser(string username, string passwordHash) {
        try
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        "SELECT id FROM USERS WHERE username = @user AND password_hash = @hash LIMIT 1;";
                    var p1 = command.CreateParameter(); p1.ParameterName = "@user"; p1.Value = username;
                    var p2 = command.CreateParameter(); p2.ParameterName = "@hash"; p2.Value = passwordHash;
                    command.Parameters.Add(p1); command.Parameters.Add(p2);
                    using (var reader = command.ExecuteReader())
                        if (reader.Read()) return reader.GetInt32(0);
                }
            }
        }
        catch (Exception ex) { Debug.LogError($"[SQLite] ValidateUser: {ex.Message}"); }
        return -1;
    }

    public bool RegisterUser(string username, string passwordHash) {
        try
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        "INSERT INTO USERS (username, password_hash) VALUES (@user, @hash);";
                    var p1 = command.CreateParameter(); p1.ParameterName = "@user"; p1.Value = username;
                    var p2 = command.CreateParameter(); p2.ParameterName = "@hash"; p2.Value = passwordHash;
                    command.Parameters.Add(p1); command.Parameters.Add(p2);
                    command.ExecuteNonQuery();
                    return true;
                }
            }
        }
        catch (Exception ex) { Debug.LogError($"[SQLite] RegisterUser: {ex.Message}"); return false; }
    }

    public bool UsernameExists(string username) {
        try
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT COUNT(1) FROM USERS WHERE username = @user;";
                    var p = command.CreateParameter(); p.ParameterName = "@user"; p.Value = username;
                    command.Parameters.Add(p);
                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }
        catch (Exception ex) { Debug.LogError($"[SQLite] UsernameExists: {ex.Message}"); return false; }
    }

    #region Inventory Methods

    public Inventory LoadInventory(int userId) {
        try
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                var items = new List<Item>();

                // Get inventory metadata
                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        "SELECT max_slots FROM INVENTORY WHERE user_id = @userId LIMIT 1;";
                    var p = command.CreateParameter();
                    p.ParameterName = "@userId";
                    p.Value = userId;
                    command.Parameters.Add(p);

                    int maxSlots = 8; // Default
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            maxSlots = reader.GetInt32(0);
                        }
                    }

                    // Get all items for this user
                    using (var itemCommand = connection.CreateCommand())
                    {
                        itemCommand.CommandText = @"
                        SELECT ii.item_definition_id, ii.amount 
                        FROM INVENTORY_ITEM ii
                        INNER JOIN INVENTORY i ON ii.inventory_id = i.id
                        WHERE i.user_id = @userId
                        ORDER BY ii.slot_index;";

                        var userParam = itemCommand.CreateParameter();
                        userParam.ParameterName = "@userId";
                        userParam.Value = userId;
                        itemCommand.Parameters.Add(userParam);

                        using (var reader = itemCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int itemDataId = reader.GetInt32(0);
                                int amount = reader.GetInt32(1);

                                Debug.Log($"[SQLite] Loading item ID: {itemDataId}, Amount: {amount}");

                                // Get ItemData from cache
                                if (itemDataCache.TryGetValue(itemDataId, out ItemData itemData))
                                {
                                    Item newItem = new Item(itemData, amount);
                                    items.Add(newItem);
                                    Debug.Log($"[SQLite] ✓ Loaded item: {itemData.itemName} x{amount}");
                                }
                                else
                                {
                                    Debug.LogError($"[SQLite] ❌ ItemData with ID {itemDataId} not found in cache!");
                                    Debug.Log($"[SQLite] Available IDs in cache: {string.Join(", ", itemDataCache.Keys)}");
                                }
                            }
                        }
                    }
                    return new Inventory(userId, items, maxSlots);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[SQLite] LoadInventory failed: {ex.Message}");
            return null;
        }
    }

    public void SaveInventory(Inventory inventory) {
        if (inventory == null) return;

        try
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                // Get or create inventory record
                int inventoryId = -1;
                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        "SELECT id FROM INVENTORY WHERE user_id = @userId;";
                    var p = command.CreateParameter();
                    p.ParameterName = "@userId";
                    p.Value = inventory.userId;
                    command.Parameters.Add(p);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            inventoryId = reader.GetInt32(0);
                        }
                    }
                }

                // Create inventory if it doesn't exist
                if (inventoryId == -1)
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText =
                            "INSERT INTO INVENTORY (user_id, max_slots) VALUES (@userId, @maxSlots);";
                        var p1 = command.CreateParameter();
                        p1.ParameterName = "@userId";
                        p1.Value = inventory.userId;
                        var p2 = command.CreateParameter();
                        p2.ParameterName = "@maxSlots";
                        p2.Value = inventory.maxSlots;
                        command.Parameters.Add(p1);
                        command.Parameters.Add(p2);
                        command.ExecuteNonQuery();
                    }

                    // Get the newly created inventory ID
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText =
                            "SELECT id FROM INVENTORY WHERE user_id = @userId;";
                        var p = command.CreateParameter();
                        p.ParameterName = "@userId";
                        p.Value = inventory.userId;
                        command.Parameters.Add(p);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                inventoryId = reader.GetInt32(0);
                            }
                        }
                    }
                }

                // Clear existing items
                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        "DELETE FROM INVENTORY_ITEM WHERE inventory_id = @inventoryId;";
                    var p = command.CreateParameter();
                    p.ParameterName = "@inventoryId";
                    p.Value = inventoryId;
                    command.Parameters.Add(p);
                    command.ExecuteNonQuery();
                }

                // Save current items
                for (int i = 0; i < inventory.itemList.Count; i++)
                {
                    var item = inventory.itemList[i];
                    if (item.itemData == null) continue;

                    Debug.Log($"[SQLite] Saving item: {item.itemData.itemName} (ID: {item.itemData.id}) x{item.amount}");

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                        INSERT INTO INVENTORY_ITEM 
                        (item_definition_id, inventory_id, slot_index, amount) 
                        VALUES (@itemDefId, @inventoryId, @slotIndex, @amount);";

                        var p1 = command.CreateParameter();
                        p1.ParameterName = "@itemDefId";
                        p1.Value = item.itemData.id;

                        var p2 = command.CreateParameter();
                        p2.ParameterName = "@inventoryId";
                        p2.Value = inventoryId;

                        var p3 = command.CreateParameter();
                        p3.ParameterName = "@slotIndex";
                        p3.Value = i;

                        var p4 = command.CreateParameter();
                        p4.ParameterName = "@amount";
                        p4.Value = item.amount;

                        command.Parameters.Add(p1);
                        command.Parameters.Add(p2);
                        command.Parameters.Add(p3);
                        command.Parameters.Add(p4);
                        command.ExecuteNonQuery();
                    }
                }

                Debug.Log($"[SQLite] Inventory saved for user {inventory.userId}. Saved {inventory.itemList.Count} items");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[SQLite] SaveInventory failed: {ex.Message}");
        }
    }

    #endregion
}