using DapperBeer.Model;
using FluentAssertions;
using DapperBeer;

namespace DapperBeerNunit.Tests;

[TestFixture]
public class Assignments3Tests : TestHelper
{
    // 3.1 Test
    [Test]
    public async Task GetAllBrouwmeestersIncludesAddressTest()
    {
        var allBrewmastersIncludeAddress = await Assignments3.GetAllBrouwmeestersIncludesAddress();
        await Verify(allBrewmastersIncludeAddress.Take(3));
    }
    
    // 3.2 Test
    [Test]
    public async Task GetAllBrewmastersWithBreweryTest()
    {
        var allBrewmastersWithBrewery = await Assignments3.GetAllBrewmastersWithBrewery();
        await Verify(allBrewmastersWithBrewery.Take(3));
    }
    
    // 3.3 Test
    [Test]
    public async Task GetAllBrewersIncludeBrewmasterTest()
    {
        var allBrewersIncludeBrewmaster = await Assignments3.GetAllBrewersIncludeBrewmaster();
        allBrewersIncludeBrewmaster.Should().HaveCount(677);
        await Verify(allBrewersIncludeBrewmaster.Take(3));
    }
    
    // 3.4 Test
    [Test]
    public async Task GetAllBeersIncludeBreweryTest()
    {
        var allBeersIncludeBrewery = await Assignments3.GetAllBeersIncludeBrewery();
        await Verify(allBeersIncludeBrewery);
    }
    
    // 3.5 Test
    [Test]
    public async Task GetAllBrewersIncludingBeersNPlus1Test()
    {
        var allBrewersIncludingBeersNPlus1 = await Assignments3.GetAllBrewersIncludingBeersNPlus1();
        allBrewersIncludingBeersNPlus1.Should().HaveCount(677);
        await Verify(allBrewersIncludingBeersNPlus1.Take(3));
    }
    
    // 3.6 Test
    [Test]
    public async Task GetAllBrewersIncludeBeersTest()
    {
        var allBrewersIncludeBeers = await Assignments3.GetAllBrewersIncludeBeers();
        allBrewersIncludeBeers.Should().HaveCount(677);
        await Verify(allBrewersIncludeBeers.Take(3));
    }
    
    // 3.7 Test
    [Test]
    public async Task GetAllBeersIncludeBreweryAndIncludeBeersInBreweryTest()
    {
        var allBeersIncludeBreweryAndIncludeBeersInBrewery = await Assignments3.GetAllBeersIncludeBreweryAndIncludeBeersInBrewery();
        await Verify(allBeersIncludeBreweryAndIncludeBeersInBrewery);
    }

    // 3.8 Test
    [Test]
    public async Task GetAllBeersServedPerCafeTest()
    {
        var data = await Assignments3.GetAllBeersServedPerCafe();
        data.Should().HaveCount(345);
        await Verify(data.Take(15));
    }
    
    // 3.9 Test
    [Test]
    public async Task GetAllBrewersIncludeBeersThenIncludeCafesTest()
    {
        var allBrewersIncludeBeersThenIncludeCafes = await Assignments3.GetAllBrewersIncludeBeersThenIncludeCafes();
        allBrewersIncludeBeersThenIncludeCafes.Should().HaveCount(677);
        List<Brewer> brewersWithBeersServedInMultipleCafes =
            allBrewersIncludeBeersThenIncludeCafes
                .Where(x => x.Beers.Any(b => b.Cafes.Count >= 2))
                .Take(1)
                .ToList();
        
        var settings = new VerifySettings();
        settings.DontIgnoreEmptyCollections();
        await Verify(brewersWithBeersServedInMultipleCafes, settings);
    }
    
    // 3.10 Test
    [Test]
    public async Task GetBeerAndBrewersByViewTest()
    {
        var result = await Assignments3.GetBeerAndBrewersByView();
        result.Should().HaveCount(1617);
        await Verify(result.Take(3));
    }
}