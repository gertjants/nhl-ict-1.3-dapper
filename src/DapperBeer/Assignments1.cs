using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DapperBeer.DTO;
using DapperBeer.Model;
using FluentAssertions;
using System.Linq;

namespace DapperBeer;

using Dapper;

public class Assignments1
{
    // 1.1 Question
    // Geef een overzicht van alle brouwers, gesorteerd op naam (alfabetisch).
    // Gebruik hiervoor de class Brewer. En de Query<Brewer>(sql) methode van Dapper.
    // Het is ook altijd een goed idee om het resultaat van de Query om te zetten naar een `List<T>`, 
    // Dit kan met de methode ToList() die je aan het einde van de Query() kan toevoegen (connection.Query(sql).ToList()).
    // Onder deze methode staat een test. Zodat je kan controleren of je query en C# code correct werkt.
    // Tip: mocht je een foutmelding krijgen, kijk dan goed naar de foutmelding en de query die je hebt geschreven.
    // Test eerst je SQL-query en kijk dan of je C# code correct werkt met de debugger.
    // Kom je er niet uit na zelf proberen,
    // vraag dan hulp (medestudent, docent). Het kan natuurlijk zijn dat de test niet correct is.
    // Dit is natuurlijk niet de bedoeling (mocht dit zo zijn laat het mij (joris) dan s.v.p. weten). 
    //
    // De test zijn gemaakt met TUnit, dit testframework is relatief nieuw.
    // De tests kan je vinden in de directory Tests.
    // Het kan zijn dat je even een setting moet aanpassen in Rider/Visual Studio. Dit staat beschreven in de README.md.
    // op https://github.com/thomhurst/TUnit
    // LET OP: je kan de testen per stuk runnen, maar ook allemaal tegelijk per Assignment.
    // Maar niet alle assignments (Assigment1.cs, Assignment2.cs, Assignment3.cs) tegelijk!
    // Deze testen zijn niet geschreven om parallel te runnen. Dit zorgt er wel voor dat de testen sneller runnen.
    // 
    // In de directory Model staan de classes die overeenkomen met de database tabellen.
    // In de directory DTO (Data Transfer Object) staan de classes die worden gebruikt als resultaat indien deze
    // niet overeenkomt met een database tabel (Model).
    public async static Task<IEnumerable<Brewer>> GetAllBrewers()
        => await DbHelper.GetConnection().QueryAsync<Brewer>(@"
SELECT 
    * 
FROM `brewer`;");

    // 1.2 Question
    // Geef een overzicht van alle bieren gesorteerd op alcohol percentage (hoog naar laag).
    // Remark: De verified file is ASC en niet DESC?
    public async static Task<IEnumerable<Beer>> GetAllBeersOrderByAlcohol()
        => await DbHelper.GetConnection().QueryAsync<Beer>(@"
SELECT 
    * 
FROM `beer` 
order by `Alcohol` desc, `Name`;");
    
    // 1.3 Question
    // Geef een overzicht van ale bieren voor een bepaald land gesorteerd op naam (alfabetisch).
    // Gebruik hiervoor de class Beer. En in je SQL-query JOIN met de tabel Brewer.
    // Gebruik de Query<Beer>(sql, new {Country = country}) methode van Dapper.
    // In je SQL-query kan je de WHERE-clause gebruiken om te filteren op land.
    // Gebruik hiervoor query parameters placeholders in je SQL-query.
    // Dit voorkomt SQL-injectie (onderwerp van les 2).
    //      WHERE brewer.Country = @Country
    // @Country is een query parameter placeholder.
    public async static Task<IEnumerable<Beer>> GetAllBeersSortedByNameForCountry(string country)
        => await DbHelper.GetConnection().QueryAsync<Beer>(@"
SELECT `Beer`.*
FROM `Beer` Beer 
INNER JOIN `Brewer` Brewer on (Brewer.BrewerId = Beer.BrewerId)
WHERE  brewer.Country = @Country
ORDER BY Beer.Name asc;", new { Country = country });
    
    // 1.4 Question
    // Tel het aantal brouwerijen. Welke methode van Dapper gebruik je (niet Query<Brewer>)?
    // Een handige website om te kijken is:
    // https://www.learndapper.com/
    // Voor deze vraag kijken specifiek naar deze pagina: https://www.learndapper.com/dapper-query
    // De received vraagt een specifieke volgorde, DESC.
    public async static Task<int> CountBrewers()
        => await DbHelper.GetConnection().ExecuteScalarAsync<int>(@"
SELECT 
    COUNT(*) 
FROM Brewer 
ORDER BY COUNT(*) DESC;");
    
    // 1.5 Question
    // Geef een overzicht van het aantal brouwerijen per land gesorteerd op aantal brouwerijen.
    // Gebruik hiervoor een aparte class NumberOfBrewersByCountry
    // Voeg hiervoor properties toe aan de class NumberOfBrewersByCountry, namelijk Country en NumberOfBreweries.
    // Gebruik de volgende SELECT-clause zodat de kolomnamen in de resultaten overeenkomen met de properties van de class NumberOfBrewersByCountry.:
    //   SELECT Country, COUNT(1) AS NumberOfBreweries
    // In de directory DTO (Data Transfer Object) staan de classes die worden gebruikt als resultaat
    // voor Queries die net overeenkomen met de database tabellen.
    public async static Task<IEnumerable<NumberOfBrewersByCountry>> NumberOfBrewersByCountry()
        => await DbHelper.GetConnection().QueryAsync<NumberOfBrewersByCountry>(@"
SELECT 
    Country,
    COUNT(*) AS NumberOfBreweries 
FROM Brewer 
GROUP BY Country
ORDER BY NumberOfBreweries DESC;");
    
    // 1.6 Question
    // Geef het bier met het hoogste alcohol percentage terug. Welke methode gebruik je van Dapper (niet Query<Beer>)?
    // Je kan in MySQL de LIMIT 1 gebruiken om 1 record terug te krijgen.
    public async static Task<Beer> GetBeerWithMostAlcohol()
        => await DbHelper.GetConnection().QuerySingleAsync<Beer>(@"
SELECT
    *
FROM Beer
ORDER BY Alcohol DESC LIMIT 1;");
    
    // 1.7 Question
    // Gegeven de brewerId geef de brouwer terug. Let op: Wat moet er gebeuren als de brouwcode niet bestaat?
    // Met andere woorden, welke Dapper methode moet je gebruiken? 
    // Brewer? is een nullable type. Dit betekent dat de waarde null kan zijn,
    // indien de brouwerij niet bestaat voor een bepaalde brewerId.
    public async static Task<Brewer?> GetBreweryByBrewerId(int brewerId)
        => await DbHelper.GetConnection().QuerySingleOrDefaultAsync<Brewer>(@"
SELECT 
    * 
FROM Brewer 
WHERE BrewerId = @BrewerId;", new { BrewerId = brewerId });

    
    // 1.8 Question
    // Gegeven de BrewerId, geef een overzicht van alle bieren van de brouwerij gesorteerd bij alcohol percentage.
    public async static Task<IEnumerable<Beer>> GetAllBeersByBreweryId(int brewerId)
        => await DbHelper.GetConnection().QueryAsync<Beer>(@"
SELECT
    *
FROM Beer 
WHERE BrewerId = @BrewerId 
ORDER BY alcohol;", new { BrewerId = brewerId});
    
    // 1.9 Question
    // Geef per cafe aan welke bieren ze schenken, sorteer op cafe naam en daarna bier naam.
    // Gebruik hiervoor de class CafeBeer (directory DTO). 
    // Voeg hiervoor properties toe aan de class CafeBeer, namelijk Beer en Cafe
    public async static Task<IEnumerable<CafeBeer>> GetCafeBeers()
     => await DbHelper.GetConnection().QueryAsync<CafeBeer>(@"
SELECT 
    Beer.Name AS Beers, 
    Cafe.Name AS CafeName 
FROM Beer 
INNER JOIN Sells ON (Sells.BeerId = Beer.BeerId) 
INNER JOIN Cafe ON (Cafe.CafeId = Sells.CafeId)
ORDER BY CafeName, Beers");
    
    // 1.10 Question
    // Hetzelfde resultaat als de vorige vraag alleen op een andere manier.
    // Geef nu een lijst van de namen van de bieren gescheiden door een komma terug in de SQL-query.
    // Je kan hiervoor de SQL-methode GROUP_CONCAT(beer.Name ORDER BY beer.Name) gebruiken in je SELECT-clause.
    // Sorteer op naam van het cafe en daarna op de namen van de bieren (deze sortering zit in de GROUP_CONCAT(beer.Name ORDER BY beer.Name)).
    // Gebruik hiervoor de class CafeBeerList, de properties zijn al toegevoegd.
    // Het probleem is dat een cafe meerdere bieren kan schenken. Hoe los je dit op?
    // Je zult wat extra code moeten schrijven in C#.
    // De truc is dat je klasse CafeBeer gebruikt voor je Query<CafeBeer> en dan 'converteren/kopiëren van de waardes' naar CafeBeerList. 
    public async static Task<List<CafeBeerList>> GetCafeBeersByList()
    {
        using var conn = DbHelper.GetConnection(); 
        var cafeBeers = await conn.QueryAsync<CafeBeer>(@"
SELECT 
    Beer.Name AS Beers, 
    Cafe.Name AS CafeName 
FROM Beer 
INNER JOIN Sells ON (Sells.BeerId = Beer.BeerId) 
INNER JOIN Cafe ON (Cafe.CafeId = Sells.CafeId)");
        
        return (from q in cafeBeers
                group q.Beers by q.CafeName into grp 
                select new CafeBeerList {
                    CafeName = grp.Key,
                    Beers = grp.ToList()
        }).ToList();
    }
    
    // 1.11 Question
    // Geef de gemiddelde waardering (score in de tabel Review) van een biertje terug gegeven de BeerId.
    public async static Task<decimal> GetBeerRating(int beerId)
        => await DbHelper.GetConnection().ExecuteScalarAsync<decimal>("SELECT AVG(score) as avg_score FROM review where BeerId=@BeerId", 
        new { 
                BeerId=beerId
            }
        );
    
    // 1.12 Question
    // Voeg een review toe voor een bier.
    public async static Task InsertReview(int beerId, decimal score)
        => await InsertReviewReturnsReviewId(beerId, score);
    
    // 1.13 Question
    // Voeg een review toe voor bier. Geef de reviewId terug.
    public async static Task<int> InsertReviewReturnsReviewId(int beerId, decimal score)
        => await DbHelper.GetConnection().ExecuteScalarAsync<int>("INSERT INTO Review (BeerId, Score) VALUES(@BeerId, @Score); SELECT LAST_INSERT_ID();", 
        new {
             
                BeerId=beerId,
                Score=score
            }
        ); 
    
    // 1.14 Question
    // Update een review voor een bepaalde reviewId.
    public static void UpdateReviews(int reviewId, decimal score)
    {
        using var conn = DbHelper.GetConnection();  
        DbHelper.GetConnection().Execute("UPDATE Review SET score=@Score where ReviewId=@ReviewId", 
        new { 
                ReviewId=reviewId,
                Score=score
            }
        );
    }
    
    // 1.15 Question 
    // Verwijder een review voor een bepaalde reviewId.
    public static void RemoveReviews(int reviewId)
    {
        using var conn = DbHelper.GetConnection(); 
        DbHelper.GetConnection().Execute("DELETE FROM Review where ReviewId=@ReviewId", new { ReviewId=reviewId});
    }
        
}