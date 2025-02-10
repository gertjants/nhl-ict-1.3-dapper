namespace ExampleFromSheets.Model;

public class Actor
{
    public int ActorId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime LastUpdate { get; set; }

    public List<Film> Films { get; set; } = new List<Film>();
}