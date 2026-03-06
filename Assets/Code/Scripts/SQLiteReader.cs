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
            // You can add more tables here: CreateTable<Player>(), etc.
            
            Debug.Log("[SQLite] Database initialized with ORM.");
        } catch(Exception ex) {
            Debug.LogError($"[SQLite] Initialization failed: {ex.Message}");
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
}
