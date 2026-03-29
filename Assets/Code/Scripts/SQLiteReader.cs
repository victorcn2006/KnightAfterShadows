using SQLite4Unity3d;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

public class SQLiteReader : MonoBehaviour {

    public static SQLiteReader instance { get; private set; }
    
    private SQLiteConnection _connection;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            InitializeDatabase();
        } else {
            Destroy(this.gameObject);
        }
    }

    private void InitializeDatabase() {
        string dbPath = Path.Combine(Application.dataPath, "MyDatabase.sqlite");
        
        try {
            _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
            
            // Migrate tables to ORM
            _connection.CreateTable<User>();
            _connection.CreateTable<ItemDefinition>();
            _connection.CreateTable<Player>();
            _connection.CreateTable<Inventari>();
            
            InitializeDefaultItems();
            
            Debug.Log("[SQLite] Database initialized with ORM.");
        } catch(Exception ex) {
            Debug.LogError($"[SQLite] Initialization failed: {ex.Message}");
        }
    }

    private void InitializeDefaultItems() {
        if (_connection.Table<ItemDefinition>().Count() == 0) {
            var defaults = new List<ItemDefinition> {
                new ItemDefinition { name = "Skull", price = 80, sprite_name = "skull", stackable = 0, max_amount = 1 },
                new ItemDefinition { name = "Helmet", price = 100, sprite_name = "helmet", stackable = 0, max_amount = 1 },
                new ItemDefinition { name = "Sword", price = 150, sprite_name = "sword", stackable = 0, max_amount = 1 },
                // Added Silk, Potion, Gem
                new ItemDefinition { name = "Silk", price = 50, sprite_name = "silk", stackable = 1, max_amount = 99 },
                new ItemDefinition { name = "Potion", price = 20, sprite_name = "potion", stackable = 1, max_amount = 99 },
                new ItemDefinition { name = "Gem", price = 500, sprite_name = "gem", stackable = 1, max_amount = 10 }
            };
            _connection.InsertAll(defaults);
            Debug.Log("[SQLite] Default items initialized.");
        }
    }

    #region User Operations

    public bool RegisterUser(string username, string password) {
        try {
            var newUser = new User {
                username = username,
                password_hash = password // In a real app, hash this!
            };
            _connection.Insert(newUser);
            return true;
        } catch (Exception ex) {
            Debug.LogError($"[SQLite] Registration failed: {ex.Message}");
            return false;
        }
    }

    public bool ValidateUser(string username, string password) {
        try {
            var user = _connection.Table<User>()
                .Where(u => u.username == username && u.password_hash == password)
                .FirstOrDefault();
            return user != null;
        } catch (Exception ex) {
            Debug.LogError($"[SQLite] Validation failed: {ex.Message}");
            return false;
        }
    }

    public List<User> GetAllUsers() {
        try {
            return _connection.Table<User>().ToList();
        } catch (Exception ex) {
            Debug.LogError($"[SQLite] Failed to get users: {ex.Message}");
            return new List<User>();
        }
    }

    public bool DeleteUser(int userId) {
        try {
            _connection.Delete<User>(userId);
            Debug.Log($"[SQLite] User with ID {userId} deleted.");
            return true;
        } catch (Exception ex) {
            Debug.LogError($"[SQLite] Delete failed: {ex.Message}");
            return false;
        }
    }

    #endregion

    #region Item Operations

    public List<ItemDefinition> GetAllItems() {
        try {
            return _connection.Table<ItemDefinition>().ToList();
        } catch (Exception ex) {
            Debug.LogError($"[SQLite] Failed to get items: {ex.Message}");
            return new List<ItemDefinition>();
        }
    }

    #endregion

    #region Player Operations

    public Player GetPlayerByUserId(int userId) {
        try {
            var player = _connection.Table<Player>().Where(p => p.user_id == userId).FirstOrDefault();
            if (player == null) {
                player = new Player {
                    user_id = userId,
                    diners = 1000 // Initial money
                };
                _connection.Insert(player);
            }
            return player;
        } catch (Exception ex) {
            Debug.LogError($"[SQLite] Failed to get/create player: {ex.Message}");
            return null;
        }
    }

    public List<Inventari> GetPlayerInventory(int playerId) {
        try {
            return _connection.Table<Inventari>().Where(i => i.player_id == playerId).ToList();
        } catch (Exception ex) {
            Debug.LogError($"[SQLite] Failed to get inventory: {ex.Message}");
            return new List<Inventari>();
        }
    }

    #endregion

    #region Shop Operations

    public bool BuyItem(int playerId, int itemId, int quantity, int totalCost, bool forceError = false) {
        try {
            _connection.BeginTransaction();

            // 1. Deduct money
            var player = _connection.Table<Player>().Where(p => p.id == playerId).FirstOrDefault();
            if (player == null) throw new Exception("Player not found.");
            if (player.diners < totalCost) throw new Exception("Insufficient funds.");
            
            player.diners -= totalCost;
            _connection.Update(player);

            // SIMULATED ERROR for Testing
            if (forceError) {
                Debug.LogWarning("[SQLite] Simulating database error during transaction...");
                throw new Exception("Simulated database error during transaction.");
            }

            // 2. Add/Update Inventory
            var existingItem = _connection.Table<Inventari>()
                .Where(i => i.player_id == playerId && i.item_id == itemId)
                .FirstOrDefault();

            if (existingItem != null) {
                existingItem.quantity += quantity;
                _connection.Update(existingItem);
            } else {
                var newItem = new Inventari {
                    player_id = playerId,
                    item_id = itemId,
                    quantity = quantity
                };
                _connection.Insert(newItem);
            }

            _connection.Commit();
            Debug.Log($"[SQLite] Buy successful. Player {playerId} bought {quantity} of item {itemId}.");
            return true;

        } catch (Exception ex) {
            _connection.Rollback();
            Debug.LogError($"[SQLite] Buy failed, rollback executed: {ex.Message}");
            return false;
        }
    }

    public bool SellItem(int playerId, int itemId, int quantity, int totalRevenue) {
        try {
            _connection.BeginTransaction();

            // 1. Check/Deduct inventory
            var existingItem = _connection.Table<Inventari>()
                .Where(i => i.player_id == playerId && i.item_id == itemId)
                .FirstOrDefault();

            if (existingItem == null || existingItem.quantity < quantity) {
                throw new Exception("Insufficient items to sell.");
            }

            existingItem.quantity -= quantity;
            if (existingItem.quantity == 0) {
                _connection.Delete(existingItem);
            } else {
                _connection.Update(existingItem);
            }

            // 2. Add money
            var player = _connection.Table<Player>().Where(p => p.id == playerId).FirstOrDefault();
            if (player == null) throw new Exception("Player not found.");
            player.diners += totalRevenue;
            _connection.Update(player);

            _connection.Commit();
            Debug.Log($"[SQLite] Sell successful. Player {playerId} sold {quantity} of item {itemId}.");
            return true;

        } catch (Exception ex) {
            _connection.Rollback();
            Debug.LogError($"[SQLite] Sell failed, rollback executed: {ex.Message}");
            return false;
        }
    }

    #endregion
}
