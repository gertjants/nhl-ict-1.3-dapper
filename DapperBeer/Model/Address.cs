namespace DapperBeer.Model;

public class Address
{
    public int AddressId { get; set; }
    public required string Street { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
}