using SQLite4Unity3d;

public class ItemDefinition 
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }

    public int stackable { get; set; }
    public int max_amount { get; set; }
    public string name { get; set; }
    public string description { get; set; }
}
