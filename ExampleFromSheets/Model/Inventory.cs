namespace ExampleFromSheets.Model;

public class Inventory
{
    public int InventoryId { get; set; }
    
    public int FilmId { get; set; }
    public Film Film { get; set; }
    
    public int StoreId { get; set; }
    // public Store Store { get; set; }
    
    public DateTime LastUpdate { get; set; }
    
    // public List<Rental> Rentals { get; set; } = new List<Rental>();
}