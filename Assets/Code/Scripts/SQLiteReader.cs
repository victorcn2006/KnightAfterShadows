using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SQLiteReader : MonoBehaviour
{
    string dbUri = "Uri=file:" + Application.dataPath + "/MyDatabase.sqlite";

    IDBConnection dbConnection = new SqliteConnection(dbUri);
}
