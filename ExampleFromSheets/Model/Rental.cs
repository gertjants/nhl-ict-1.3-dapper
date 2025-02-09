namespace ExampleFromSheets.Model;

public class Rental
{
    public int RentalId { get; set; }
    public DateTime RentalDate { get; set; }
    
    public int InventoryId { get; set; }
    public Inventory Inventory { get; set; }
    
    public int CustomerId { get; set; }
    // public Customer Customer { get; set; }
    public DateTime ReturnDate { get; set; }
    public int StaffId { get; set; }
    // public Staff Staff { get; set; }
    public DateTime LastUpdate { get; set; }
}