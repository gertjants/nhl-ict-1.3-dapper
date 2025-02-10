namespace DapperBeer.DTO;

public class BrewerBeerBrewmaster
{
    public required string BeerName { get; init; }
    public required string BrewerName { get; init; }
    public string? BrewmasterName { get; init; }
}