namespace DapperBeer.Model;

public class Brewer
{
    public int BrewerId { get; set; }
    public required string Name { get; set; }
    public required string Country { get; set; }

    public List<Beer> Beers { get; set; } = new List<Beer>();
    public Brewmaster? Brewmaster { get; set; }
}