using SQLite4Unity3d;

public class Inventari 
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }

    public int player_id { get; set; }
    public int item_id { get; set; }
    public int quantity { get; set; }
}
