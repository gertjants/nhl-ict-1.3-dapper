using System.Data;
using Dapper;
using ExampleFromSheets.Model;
using FluentAssertions;
using MySqlConnector;

namespace ExampleFromSheets;

public class Relationships
{
    // 1 to 1 relationship between Customer and Address
    // A customer (one) has one address (one).
    // We start with this query from the 1 side (Customer) and include the 1 side (Address).
    public List<Customer> GetCustomerIncludeAddress()
    {
        string sql =
            """
            SELECT 
                c.customer_id AS CustomerId,
                c.store_id AS StoreId,
                c.first_name AS FirstName,
                c.last_name AS LastName,
                c.email AS Email,
                c.address_id AS AddressId,
                c.active AS Active,
                c.create_date AS CreateDate,
                c.last_update AS LastUpdate,
                '' as 'AddressSplit',
                a.address_id AS AddressId,
                a.address AS Address1,
                a.address2 AS Address2,
                a.district AS District,
                a.city_id AS CityId,
                a.postal_code AS PostalCode,
                a.phone AS Phone,
                a.last_update AS LastUpdate
            FROM customer c 
                JOIN address a ON c.address_id = a.address_id
            ORDER BY c.last_name, c.first_name
            Limit 10
            """;

        using IDbConnection connection = DbHelper.GetConnection();
        List<Customer> customers = connection.Query<Customer, Address, Customer>(
                sql,
                map: (customer, address) =>
                {
                    customer.Address = address;
                    return customer;
                },
                splitOn: "AddressSplit")
            .ToList();
        return customers;
    }

    [Test]
    public async Task GetCustomerIncludeAddressTest()
    {
        List<Customer> customers = GetCustomerIncludeAddress();
        customers.Should().HaveCount(10);
        await Verify(customers.Take(3));
    }

    // 1 to N relationship between Customer and Store
    // So a store (one) rents movies to multiple (N) customers.
    // We start with this query from the N side (Customer) and include the 1 side (Store). 
    // 
    // In this case, we can use a JOIN to get the store information for each customer
    // with the associated store (IncludeStore).
    // To let dapper know how to map the store information to the Store property of the Customer class,
    // we need to use the splitOn (splitOn: 'StoreSplit') parameter to tell dapper where to split the result set (records obtained from the query).
    // We can use a dummy column name (StoreSplit) to split the result set.
    //
    // In the map function, we can then assign the store information to the Store property of the Customer class.
    // So a store can be associated with multiple customers.
    // So we want to reuse the store if it is already loaded. (we are not creating a new store object for each customer)
    // The trick is to use a dictionary to store the store information and reuse it if it is already loaded.
    // The key for the dictionary is the StoreId.
    //      Dictionary<int, Store> storeDictionary = new DictionaryStore();
    // Every time we get a customer / store combination in the map function,
    // we check if the store is already loaded in the dictionary. If it is, we reuse it.
    // Otherwise, we add it to the dictionary.
    //
    // The query returns 10 customers and their associated store (LIMIT 10)
    public List<Customer> GetCustomerIncludeStore()
    {
        string sql = @"SELECT
                    c.customer_id AS CustomerId,
                    c.store_id AS StoreId,
                    c.first_name AS FirstName,
                    c.last_name AS LastName,
                    c.email AS Email,
                    c.address_id AS AddressId,
                    c.active AS Active,
                    c.create_date AS CreateDate,
                    c.last_update AS LastUpdate,
                    
                    '' AS 'StoreSplit',
                    
                    s.store_id AS StoreId,
                    s.manager_staff_id AS ManagerStaffId,
                    s.address_id AS AddressId,
                    s.last_update AS LastUpdate
                    FROM customer c JOIN store s ON c.store_id = s.store_id
                    ORDER BY c.customer_id
                    LIMIT 10";

        Dictionary<int, Store> storeDictionary = new Dictionary<int, Store>();

        using IDbConnection connection = DbHelper.GetConnection();
        List<Customer> customers = connection.Query<Customer, Store, Customer>(
            sql,
            map: (customer, store) =>
            {
                if (storeDictionary.ContainsKey(store.StoreId))
                {
                    store = storeDictionary[store.StoreId];
                }
                else
                {
                    storeDictionary.Add(store.StoreId, store);
                }

                //store maintains a list of customers
                store.Customers.Add(customer);

                customer.Store = store;
                return customer;
            },
            splitOn: "StoreSplit").ToList();
        return customers;
    }

    [Test]
    public async Task TestGetCustomerIncludeStore()
    {
        List<Customer> customers = GetCustomerIncludeStore();
        customers.Should().HaveCount(10);

        // Check if we reuse the store object and not create a new one for each customer
        // in other words is store object is reused correctly by maintaining 
        customers[0].Store.Should().BeSameAs(customers[1].Store);

        await Verify(customers.Take(3));
    }

    // 1 to N relationship between Store and Customer
    // So a store (one) rents movies to multiple (N) customers.
    // We start with this query from the 1 side (Store) and include the N side (Customer).
    [Test]
    public List<Store> GetStoreIncludeCustomers()
    {
        string sql =
            """
            SELECT
                s.store_id AS StoreId,
                s.manager_staff_id AS ManagerStaffId,
                s.address_id AS AddressId,
                s.last_update AS LastUpdate,
                
                '' AS 'CustomerSplit',
            
                c.customer_id AS CustomerId,
                c.store_id AS StoreId,
                c.first_name AS FirstName,
                c.last_name AS LastName,
                c.email AS Email,
                c.address_id AS AddressId,
                c.active AS Active,
                c.create_date AS CreateDate,
                c.last_update AS LastUpdate
            FROM store s JOIN customer c ON s.store_id = c.store_id
            ORDER BY c.customer_id
            """;

        Dictionary<int, Store> storeDictionary = new Dictionary<int, Store>();
        using IDbConnection connection = DbHelper.GetConnection();
        List<Store> stores = connection.Query<Store, Customer, Store>(
                sql,
                map: (store, customer) =>
                {
                    if (storeDictionary.ContainsKey(store.StoreId))
                    {
                        store = storeDictionary[store.StoreId];
                    }
                    else
                    {
                        storeDictionary.Add(store.StoreId, store);
                    }

                    store.Customers.Add(customer);
                    return store;
                },
                splitOn: "CustomerSplit")
            .Distinct() // remove duplicates, remember the SQL returns each store, customer combination
            .ToList();
        return stores;
    }

    [Test]
    public async Task TestGetStoreIncludeCustomers()
    {
        List<Store> stores = GetStoreIncludeCustomers();
        stores.Should().HaveCount(2);
        await Verify(stores);
    }

    // N to N relationship between Actor and Film
    public List<Actor> GetActorIncludeFilm()
    {
        string sql =
            """
            SELECT
                a.actor_id AS ActorId,
                a.first_name AS FirstName,
                a.last_name AS LastName,
                a.last_update AS LastUpdate,
                '' AS 'FilmSplit',
                f.film_id AS FilmId,
                f.title AS Title,
                f.description AS Description,
                f.release_year AS ReleaseYear,
                f.language_id AS LanguageId,
                f.rental_duration AS RentalDuration,
                f.rental_rate AS RentalRate,
                f.length AS Length,
                f.replacement_cost AS ReplacementCost,
                f.rating AS Rating,
                f.special_features AS SpecialFeatures,
                f.last_update AS LastUpdate
            FROM actor a
               JOIN film_actor fa ON a.actor_id = fa.actor_id
                   JOIN film f ON fa.film_id = f.film_id
            ORDER BY a.last_name, a.first_name, f.title
            """;

        using IDbConnection connection = DbHelper.GetConnection();
        Dictionary<int, Actor> actorDictionary = new Dictionary<int, Actor>();
        List<Actor> actors = connection.Query<Actor, Film, Actor>(
                sql,
                map: (actor, film) =>
                {
                    if (actorDictionary.ContainsKey(actor.ActorId))
                    {
                        actor = actorDictionary[actor.ActorId];
                    }
                    else
                    {
                        actorDictionary.Add(actor.ActorId, actor);
                    }

                    actor.Films.Add(film);
                    return actor;
                }, splitOn: "FilmSplit")
            .Distinct() // remove duplicates, remember the SQL return each actor, film combination
            .ToList();
        return actors;
    }

    [Test]
    public async Task TestGetActorIncludeFilm()
    {
        List<Actor> actors = GetActorIncludeFilm();
        actors.Should().HaveCount(200);
        await Verify(actors.Take(3));
    }


    // N to N relationship between Actor and Film
    // Same as previous example but with a limit of 3 actors, in the previous example we got all actors
    // we can't use LIMIT in the query because the limit is on the combination of actor and film
    // We can limit the number of actors to 3
    // We can use a subquery to limit the number of actors to 3
    public List<Actor> ActorWithFilmsFirst3Actors()
    {
        string sql = """
                     SELECT
                         a.actor_id AS ActorId,
                         a.first_name AS FirstName,
                         a.last_name AS LastName,
                         a.last_update AS LastUpdate,
                         '' AS 'FilmSplit',
                         f.film_id AS FilmId,
                         f.title AS Title,
                         f.description AS Description,
                         f.release_year AS ReleaseYear,
                         f.language_id AS LanguageId,
                         f.rental_duration AS RentalDuration,
                         f.rental_rate AS RentalRate,
                         f.length AS Length,
                         f.replacement_cost AS ReplacementCost,
                         f.rating AS Rating,
                         f.special_features AS SpecialFeatures,
                         f.last_update AS LastUpdate
                     FROM (SELECT * FROM actor LIMIT 3) a -- this is a subquery to limit the actors to 3
                              JOIN film_actor fa ON a.actor_id = fa.actor_id
                              JOIN film f ON fa.film_id = f.film_id
                     ORDER BY a.last_name, a.first_name, f.title
                     """;

        using IDbConnection connection = DbHelper.GetConnection();
        Dictionary<int, Actor> actorDictionary = new Dictionary<int, Actor>();
        List<Actor> actors = connection.Query<Actor, Film, Actor>(
                sql,
                map: (actor, film) =>
                {
                    if (actorDictionary.ContainsKey(actor.ActorId))
                    {
                        actor = actorDictionary[actor.ActorId];
                    }
                    else
                    {
                        actorDictionary.Add(actor.ActorId, actor);
                    }

                    actor.Films.Add(film);
                    return actor;
                },
                splitOn: "FilmSplit")
            .Distinct() // remove duplicates, remember the SQL return each actor, film combination
            .ToList();
        return actors;
    }
    
    [Test]
    public async Task TestActorWithFilmsFirst3Actors()
    {
        List<Actor> actors = ActorWithFilmsFirst3Actors();
        actors.Should().HaveCount(3);
        await Verify(actors);
    }

    // Multiple relationships
    // A customer rents movies (1 to N) and some customers haven't rented a movies (this is why a left-join is used).
    // In this database all customer have rented movies. But if we create a new customer without rentals, we can see the difference.
    // We can use a LEFT JOIN to get all customers and their rentals.
    // Take a close look at the type parameter of the Query method.
    //      connection.Query<Customer, Rental?, Inventory, Film, Customer>(...)
    // Some customers don't have rentals (1 to N, N = 0..*).
    // This is because we are using a LEFT JOIN to get the rentals.
    // Dapper can't handle this automatically, so we need to use a trick to get this working.
    // if (rental.RentalId != 0) // check if rental exists, some customers don't have rentals!!
    // Because we are joining multiple tables, the parameter splitOn: "CustomerSplit, RentalSplit, InventorySplit".
    
    // A small note: a customer can rent the same movie multiple times. A movie will be in the rental list multiple times.
    // Do you now how to remove those duplicates?
    
    // To see the result of this query with a customer without rentals, you can insert a new customer without rentals.
    // Don't forget to remove otherwise the test will fail.
    public List<Customer> GetCustomerWithThereRentedMovies()
    {
        string sql =
            """
            SELECT 
                c.customer_id AS CustomerId,
                c.store_id AS StoreId,
                c.first_name AS FirstName,
                c.last_name AS LastName,
                c.email AS Email,
                c.address_id AS AddressId,
                c.active AS Active,
                c.create_date AS CreateDate,
                c.last_update AS LastUpdate,
                '' AS 'CustomerSplit',
                -- r.rental_id AS Id,
                r.rental_id AS RentalId,
                r.rental_date AS RentalDate,
                r.inventory_id AS InventoryId,
                r.customer_id AS CustomerId,
                r.return_date AS ReturnDate,
                r.staff_id AS StaffId,
                r.last_update AS LastUpdate,
                '' AS 'RentalSplit',
                -- i.inventory_id AS Id,
                i.inventory_id AS InventoryId,
                i.film_id AS FilmId,
                i.store_id AS StoreId,
                i.last_update AS LastUpdate,
                '' AS 'InventorySplit',
                -- f.film_id AS Id,
                f.film_id AS FilmId,
                f.title AS Title,
                f.description AS Description,
                f.release_year AS ReleaseYear,
                f.language_id AS LanguageId,
                f.rental_duration AS RentalDuration,
                f.rental_rate AS RentalRate,
                f.length AS Length,
                f.replacement_cost AS ReplacementCost,
                f.rating AS Rating,
                f.special_features AS SpecialFeatures,
                f.last_update AS LastUpdate
            FROM customer c 
                    LEFT JOIN rental r ON c.customer_id = r.customer_id
                        LEFT JOIN inventory i ON r.inventory_id = i.inventory_id
                            LEFT JOIN film f ON i.film_id = f.film_id
            ORDER BY c.customer_id, f.title
            """;
        
        IDbConnection connection = DbHelper.GetConnection();
        Dictionary<int, Customer> customerDictionary = new Dictionary<int, Customer>();
        List<Customer> customers = connection.Query<Customer, Rental?, Inventory, Film, Customer>(
            sql,
            map: (customer, rental, inventory, film) =>
            {
                //add customer to dictionary if it doesn't exist, otherwise return existing customer
                customer = customerDictionary.GetOrAdd(customer.CustomerId, customer); 
                
                if (rental is not null) // check if rental exists, some customers don't have rentals!!
                {
                    rental.Inventory = inventory;
                    inventory.Film = film;

                    customer.Rentals.Add(rental);
                }

                return customer;
            },  splitOn: "RentalId, RentalSplit, InventorySplit")  //the split on parameter is used to split the result set,
                                                                   //note that multiple columns are used (seperated by a comma)
            .Distinct() // don't forget to remove duplicates (each customer, rental, inventory, film combination)
            .ToList();

        return customers;
    }
    
    [Test]
    public async Task TestCustomersWithRentedMovies()
    {
        List<Customer> customers = GetCustomerWithThereRentedMovies();
        customers.Should().HaveCount(599);
        await Verify(customers.Take(3));
    }
}