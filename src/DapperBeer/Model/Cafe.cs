namespace DapperBeer.Model;

public class Cafe
{
    public int CafeId { get; init; }
    public required string Name { get; init; }
    public required string Address { get; init; }
    public required string City { get; init; }
    
    public List<Beer> Beers { get; set; } = new List<Beer>();
}