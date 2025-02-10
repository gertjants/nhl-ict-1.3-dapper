namespace ExampleFromSheets.Model;

public class City
{
    public int CityId { get; set; }
    public string Name { get; set; }
    public int CountryId { get; set; }
    public Country Country { get; set; }
    public DateTime LastUpdate { get; set; }
}