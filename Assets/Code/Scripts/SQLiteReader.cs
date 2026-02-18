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
                CreateItemTable(connection);
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
            name TET, 
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
                element 
            )";

            command.ExecuteNonQuery();
        }
    }

    private void CreateInventory(IDbConnection connection){
        using(IDbCommand command = connection.CreateCommand()){
            command.CommandText = 
            @"CREATE TABLE IF NOT EXISTS INVENTORY(
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            user_id INTEGER FOREIGN KEY,
            max_slots INTEGER)";
        }
    }

    private void CreateItems(IDbConnection connection) {
        using (IDbCommand command = connection.CreateCommand()){
            command.CommandText =
            @"CREATE TABLE IF NOT EXISTS INVENTORY_ITEM(
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            item_definition_id INTEGER FOREIGN KEY,
            inventory_id INTEGER FOREIGN KEY,
            slot_index INTEGER,
            amount INTEGER)";
        }
    }
    #endregion
    public enum Element { 
        Fire,
        Wind,
        Dirt,
        Water
    }

    private void CreateUsers(IDbConnection connection) {
        using (IDbCommand command = connection.CreateCommand()) {
            command.CommandText =
            @"CREATE TABLE IF NOT EXISTS USERS(
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            username TEXT UNIQUE NOT NULL,
            password_hash TEXT NOT NULL)";
        }
    }
    public class ItemDefinition {
        public int Id;
        public bool Stackable;
        public int MaxAmount;
        public string Name;
        public string Description;
    }
}
