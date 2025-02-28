// using DapperBeer.DTO;
// using DapperBeer.Model;
// using FluentAssertions;

// namespace DapperBeer.Tests;

// public class Assignments2Tests : TestHelper
// {
//     // 2.1 Test
//     [Test]
//     public void GetBeersByCountryWithSqlInjectionTest()
//     {
//         List<string> beers = Assignments2.GetBeersByCountryWithSqlInjection("BEL");
//         beers.Should().HaveCount(296);
        
//         //sql injection test, we krijgen nu alle bieren terug 86 stuks i.p.v. 7
//         List<string> allBeersSQlInjection = Assignments2.GetBeersByCountryWithSqlInjection("BEL' OR '1' = '1");
//         allBeersSQlInjection.Should().HaveCount(1617);
//     }
    
//     // 2.2 Test
//     [Test]
//     public async Task GetAllBeersByCountryTest()
//     {
//         List<string> beers = Assignments2.GetAllBeersByCountry("BEL");
//         beers.Should().HaveCount(296);
        
//         List<string> allBeers = Assignments2.GetAllBeersByCountry(null);
//         allBeers.Should().HaveCount(1617);

//         await Verify(allBeers.Take(3));
//     }
    
//     // 2.3 Test
//     [Test]
//     public void GetAllBeersByCountryAndMinAlcoholTest()
//     {
//         Assignments2.GetAllBeersByCountryAndMinAlcohol("BEL", 5.5m).Should().HaveCount(213);
//         Assignments2.GetAllBeersByCountryAndMinAlcohol(minAlcohol: 5.5m).Should().HaveCount(626);
//         Assignments2.GetAllBeersByCountryAndMinAlcohol(country: "BEL").Should().HaveCount(296);
//         Assignments2.GetAllBeersByCountryAndMinAlcohol().Should().HaveCount(1617);
//     }
    
//     // 2.4 Test
//     [Test]
//     public void GetAllBeersByCountryAndTypeTest()
//     {
//         Assignments2.GetAllBeersByCountryAndMinAlcoholOrderByWithSqlBuilder("BEL", 5.5m).Should().HaveCount(213);
//         Assignments2.GetAllBeersByCountryAndMinAlcoholOrderByWithSqlBuilder(minAlcohol: 5.5m).Should().HaveCount(626);
//         Assignments2.GetAllBeersByCountryAndMinAlcoholOrderByWithSqlBuilder(country: "BEL").Should().HaveCount(296);
//         Assignments2.GetAllBeersByCountryAndMinAlcoholOrderByWithSqlBuilder().Should().HaveCount(1617);
        
//         IEnumerable<string> orderByName = Assignments2.GetAllBeersByCountryAndMinAlcoholOrderByWithSqlBuilder(minAlcohol: 10, country: "BEL", orderBy: "beer.Name").Take(2);
//         orderByName.Should().BeEquivalentTo(["ADA 10", "BIERE DU BOUCANIER"]);
//         IEnumerable<string> orderByAlcohol = Assignments2.GetAllBeersByCountryAndMinAlcoholOrderByWithSqlBuilder(orderBy: "Alcohol").Take(2);
//         orderByAlcohol.Should().BeEquivalentTo(["GULL", "BERLINER WEISSE" ]);
//         IEnumerable<string> orderByType = Assignments2.GetAllBeersByCountryAndMinAlcoholOrderByWithSqlBuilder(country: "BEL", orderBy: "Type").Take(2);
//         orderByType.Should().BeEquivalentTo(["MAREDSOUS 8", "FLOREFFE" ]);
//     }
    
//     // 2.5 Test  
//     [Test]
//     public async Task GetAllBeerNamesWithBreweryAndBrewmasterTest()
//     {
//         List<BrewerBeerBrewmaster> result = Assignments2.GetAllBeerNamesWithBreweryAndBrewmaster();
//         result.Should().HaveCount(152);
//         await Verify(result.Take(3));
//     }
    
//     // 2.6 Test
//     [Test]
//     public async Task GetBeersByCountryAndTypeTest()
//     {
//         Assignments2.BeerFilter filter = new Assignments2.BeerFilter { Country = "BEL", Type = "LAGER, AMBE" };
//         List<Beer> beers1 = Assignments2.GetBeersByCountryAndType(filter);
//         beers1.Should().HaveCount(0);
        
//         filter = new Assignments2.BeerFilter { Country = "BEL", PageSize = 5, PageIndex = 2};
//         List<Beer> beers2 = Assignments2.GetBeersByCountryAndType(filter);
//         beers2.Should().HaveCount(5);
        
//         filter = new Assignments2.BeerFilter { OrderBy = "beer.Alcohol DESC" };
//         List<Beer> beers3 = Assignments2.GetBeersByCountryAndType(filter);
//         beers3.Should().HaveCount(10);
        
//         filter = new Assignments2.BeerFilter { };
//         List<Beer> beers4 = Assignments2.GetBeersByCountryAndType(filter);
//         beers4.Should().HaveCount(10);
        
//         List<Beer> testBeers = beers1.Take(3)
//             .Concat(beers2.Take(3))
//             .Concat(beers3.Take(3))
//             .Concat(beers4.Take(3))
//             .ToList();
        
//         await Verify(testBeers);
//     }
// }