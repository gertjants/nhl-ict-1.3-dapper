namespace DapperBeer.DTO;

public class CafeBeerList
{
    public required string CafeName { get; set; }
    public List<string> Beers { get; set; } = new List<string>();
}