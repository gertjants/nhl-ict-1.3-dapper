namespace DapperBeer.Model;

public class NumberOfBrewersByCountry
{
    public required string Country { get; init; }
    public required int NumberOfBreweries { get; init; }
}