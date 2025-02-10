namespace ExampleFromSheets.Model;

public class Store
{
    public int StoreId { get; set; }
    public int ManagerStaffId { get; set; }
    public int AddressId { get; set; }
    public DateTime LastUpdate { get; set; }

    public List<Customer> Customers { get; set; } = new List<Customer>();
}