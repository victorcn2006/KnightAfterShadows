using SQLite4Unity3d;

public class User 
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }

    [Unique]
    public string username { get; set; }

    public string password_hash { get; set; }
}
