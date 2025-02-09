namespace DapperBeer.Model;

public class Beer
{
    public int BeerId { get; init; }
    public required string Name { get; init; }
    public required string Type { get; init; }
    public required string Style { get; init; }
    public required decimal Alcohol { get; init; }
    public int BrewerId { get; init; }
    
    public Brewer? Brewer { get; set; }

    public List<Cafe> Cafes { get; set; } = new List<Cafe>();
}