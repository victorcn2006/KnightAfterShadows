using SQLite4Unity3d;

public class Player 
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }

    [Unique]
    public int user_id { get; set; }

    public int diners { get; set; }
}
