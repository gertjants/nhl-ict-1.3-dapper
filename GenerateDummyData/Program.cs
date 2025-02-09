// See https://aka.ms/new-console-template for more information

using Bogus;
using Dapper;
using DapperBeer;
using DapperBeer.Model;
using DapperBieren.Model;

using var connection = DbHelper.GetConnection();
var brewers = connection.Query<Brewer>("SELECT * FROM Brewer ORDER BY BrewerId").ToList();

var seed = 8675309;
Randomizer.Seed = new Random(seed);

var addressFaker = new Faker<Address>();
addressFaker.RuleFor(address => address.Street, f => f.Address.StreetAddress());
addressFaker.RuleFor(address => address.City, f => f.Address.City());
addressFaker.RuleFor(address => address.Country, f => f.Address.Country());


var brewmasterFaker = new Faker<Brewmaster>();
brewmasterFaker.RuleFor(bm => bm.Name, f => f.Name.FullName());
brewmasterFaker.RuleFor(bm => bm.Address, f => addressFaker);

var brewMasters = brewmasterFaker.Generate(brewers.Count / 3);
int brewMasterIndex = 0;
var faker1 = new Faker();
foreach (var brewer in brewers)
{
    if (faker1.Random.Double() <= 0.1)
    {
        var bm = brewMasters[brewMasterIndex++];

        var addressId = connection.ExecuteScalar<int>(
            sql: """
                 INSERT INTO Address (Street, City, Country) 
                 VALUES (@Street, @City, @Country); 
                 SELECT LAST_INSERT_ID();
                 """,
            param:
                new { bm.Address.Street, bm.Address.City, bm.Address.Country });

        _ = connection.Execute(
            """
            INSERT INTO Brewmaster (Name, BrewerId, AddressId) VALUES (@Name, @BrewerId, @AddressId);
            """,
            param: new { Name = bm.Name, BrewerId =brewer.BrewerId, AddressId = addressId });
    }
}