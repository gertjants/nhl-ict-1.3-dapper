namespace ExampleFromSheets.Model;

public class Customer
{
    public int CustomerId { get; set; }
    
    public int StoreId { get; set; }
    public Store Store { get; set; }
    
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    
    public int AddressId { get; set; }
    public Address Address { get; set; }
    
    public int Active { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime LastUpdate { get; set; }
    
    // Not a real table in the database
    public List<Rental> Rentals { get; set; } = new List<Rental>();
}