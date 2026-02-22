using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class SQLiteReader : MonoBehaviour {

    public static SQLiteReader instance { get; private set; }
    string dbPath => "URI=file:" + Application.dataPath + "/MyDatabase.sqlite";

    #region Initialization

    private void Awake() {
        if (instance == null) instance = this;
        else Destroy(this.gameObject);

        try {
            //using destroys the object and close the connection when it's finished instead of using Dispose
            using (var connection = GetConnection()) { 
                connection.Open();
                CreateItemTable(connection);
                CreatePlayer(connection);
                CreateInventory(connection);
                CreateInventoryItems(connection);
                CreateUsers(connection);
            }
        } catch(Exception ex) {
            Debug.LogError($"[SQLite] Initialization failed: {ex.Message}");
        }
    }

    private IDbConnection GetConnection(){
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
        using (IDbCommand command = connection.CreateCommand()) {
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
        using (IDbCommand command = connection.CreateCommand()) {
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

    private void CreateInventory(IDbConnection connection){
        using(IDbCommand command = connection.CreateCommand()){
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
        using (IDbCommand command = connection.CreateCommand()) {
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
        try {
            using (var connection = GetConnection()) {
                connection.Open();
                using (var command = connection.CreateCommand()) {
                    command.CommandText =
                        "SELECT id FROM USERS WHERE username = @user AND password_hash = @hash LIMIT 1;";
                    var p1 = command.CreateParameter(); p1.ParameterName = "@user"; p1.Value = username;
                    var p2 = command.CreateParameter(); p2.ParameterName = "@hash"; p2.Value = passwordHash;
                    command.Parameters.Add(p1); command.Parameters.Add(p2);
                    using (var reader = command.ExecuteReader())
                        if (reader.Read()) return reader.GetInt32(0);
                }
            }
        } catch (Exception ex) { Debug.LogError($"[SQLite] ValidateUser: {ex.Message}"); }
        return -1;
    }

    public bool RegisterUser(string username, string passwordHash) {
        try {
            using (var connection = GetConnection()) {
                connection.Open();
                using (var command = connection.CreateCommand()) {
                    command.CommandText =
                        "INSERT INTO USERS (username, password_hash) VALUES (@user, @hash);";
                    var p1 = command.CreateParameter(); p1.ParameterName = "@user"; p1.Value = username;
                    var p2 = command.CreateParameter(); p2.ParameterName = "@hash"; p2.Value = passwordHash;
                    command.Parameters.Add(p1); command.Parameters.Add(p2);
                    command.ExecuteNonQuery();
                    return true;
                }
            }
        } catch (Exception ex) { Debug.LogError($"[SQLite] RegisterUser: {ex.Message}"); return false; }
    }

    public bool UsernameExists(string username) {
        try {
            using (var connection = GetConnection()) {
                connection.Open();
                using (var command = connection.CreateCommand()) {
                    command.CommandText = "SELECT COUNT(1) FROM USERS WHERE username = @user;";
                    var p = command.CreateParameter(); p.ParameterName = "@user"; p.Value = username;
                    command.Parameters.Add(p);
                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        } catch (Exception ex) { Debug.LogError($"[SQLite] UsernameExists: {ex.Message}"); return false; }
    }
}
