namespace DapperBeer.Model;

public class Brewmaster
{
    public int BrewmasterId { get; set; }
    public required string Name { get; set; }
    
    public required Address Address { get; set; }
    public Brewer? Brewer { get; set; }
}