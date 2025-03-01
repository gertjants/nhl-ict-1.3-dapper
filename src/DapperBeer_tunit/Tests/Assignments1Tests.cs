using System.Data;
using Dapper;
using DapperBeer.DTO;
using DapperBeer.Model;
using FluentAssertions;

namespace DapperBeer.Tests;

public class Assignments1Tests : TestHelper
{
    // 1.1 Test
    [Test]
    public async Task GetAllBrewersTest()
    {
        var brewers = await Assignments1.GetAllBrewers();
        
        brewers.Should().HaveCount(677);

        await Verify(brewers.Take(3));
    }
    
    // 1.2 Test
    [Test]
    public async Task GetAllBeersOrderByAlcoholTest()
    {
        var beers = await Assignments1.GetAllBeersOrderByAlcohol();

        beers.Should().HaveCount(1617);

        await Verify(beers.Take(3));
    }
    
    // 1.3 Test
    [Test]
    public async Task GetAllBeersSortedByNameForCountryTest()
    {
        var beers = await Assignments1.GetAllBeersSortedByNameForCountry("BEL");

        beers.Should().HaveCount(296);

        await Verify(beers.Take(3));
    }
    
    // 1.4 Test
    [Test]
    public async Task CountBrewersTest()
    {
        int breweryCount = await Assignments1.CountBrewers();

        breweryCount.Should().Be(677);
    }
    
    // 1.5 Test
    [Test]
    public async Task NumberOfBrewersByCountryTest()
    {
        var numberOfBrewersByCountries = await Assignments1.NumberOfBrewersByCountry();

        numberOfBrewersByCountries.Should().HaveCount(46);

        await Verify(numberOfBrewersByCountries.Take(3));
    }
    
    // 1.6 Test
    [Test]
    public async Task GetBeerWithMostAlcoholTest()
    {
        Beer beer = await Assignments1.GetBeerWithMostAlcohol();

        beer.Name.Should().Be("XIAOYU");

        await Verify(beer);
    }
    
    // 1.7 Test
    [Test]
    public async Task GetBreweryByBrewerIdTest()
    {
        Brewer? brewer = await Assignments1.GetBreweryByBrewerId(689);

        brewer.Should().NotBeNull();
        brewer!.Name.Should().Be("AFFLIGEM");
        
        Brewer? brewerNull = await Assignments1.GetBreweryByBrewerId(-1);
        brewerNull.Should().BeNull();
        
        await Verify(brewer);
    }
    
    // 1.8 Test
    [Test]
    public async Task GetAllBeersByBreweryIdTest()
    {
        var beers = await Assignments1.GetAllBeersByBreweryId(689);

        beers.Should().HaveCount(2);

        await Verify(beers);
    }
    
    // 1.9 Test
    [Test]
    public async Task GetCafeBeersTest()
    {
        var cafeBeers = await Assignments1.GetCafeBeers();

        cafeBeers.Should().HaveCount(754);

        await Verify(cafeBeers.Take(3));
    }
    
    // 1.10 Test
    [Test]
    public async Task GetCafeBeersByListTest()
    {
        var cafeBeerList = await Assignments1.GetCafeBeersByList();

        cafeBeerList.Should().HaveCount(145);

        await Verify(cafeBeerList.Take(3));
    }
    
    // 1.11 Test
    [Test]
    public async Task<decimal> GetBeerRatingTest()
    {
        decimal rating = await Assignments1.GetBeerRating(338);
        rating.Should().Be(4.5m);
        return rating;
    }
    
    // 1.12 Test
    [Test]
    [NotInParallel]
    public async Task InsertReview()
    {
        await Assignments1.InsertReview(339, 4.5m);
        await Assignments1.InsertReview(339, 5.0m);
        
        decimal rating = await Assignments1.GetBeerRating(339);
        rating.Should().Be(4.75m);
    }
    
    // 1.13 Test
    [Test]
    [NotInParallel]
    public async Task UpdateReviewTest()
    {
        int reviewId = await Assignments1.InsertReviewReturnsReviewId(338, 4.5m);
        reviewId.Should().Be(5);
        
        Assignments1.UpdateReviews(reviewId, 5.0m);
        
        decimal rating = await Assignments1.GetBeerRating(338);
        rating.Should().Be(4.75m);
    }
    
    // 1.14 Test
    [Test]
    [NotInParallel]
    public async Task RemoveReviewTest()
    {        
        int reviewId = await Assignments1.InsertReviewReturnsReviewId(338, 5.0m);
        reviewId.Should().Be(4);
        
        Assignments1.RemoveReviews(reviewId);
        
        decimal rating = await Assignments1.GetBeerRating(338);
        rating.Should().Be(4.5m);
    }
}