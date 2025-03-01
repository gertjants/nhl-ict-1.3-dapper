using System.Data;
using Dapper;
using DapperBeer.DTO;
using DapperBeer.Model;
using FluentAssertions;

namespace DapperBeer;

public class Assignments3
{
    // 3.1 Question
    // Tip: Kijk in voorbeelden en sheets voor inspiratie.
    // Deze staan in de directory ExampleFromSheets/Relationships.cs. 
    // De sheets kan je vinden op: https://slides.com/jorislops/dapper/
    // Kijk niet te veel naar de voorbeelden van relaties op https://www.learndapper.com/relationships
    // Deze aanpak is niet altijd de manier de gewenst is!
    
    // 1 op 1 relatie (one-to-one relationship)
    // Een brouwmeester heeft altijd 1 adres. Haal alle brouwmeesters op en zorg ervoor dat het address gevuld is.
    // Sorteer op naam.
    // Met andere woorden een brouwmeester heeft altijd een adres (Property Address van type Address), zie de klasse Brewmaster.
    // Je kan dit doen door een JOIN te gebruiken.
    // Je zult de map functie in Query<Brewmaster, Address, Brewmaster>(sql, map: ...) moeten gebruiken om de Address property van Brewmaster te vullen.
    // Kijk in voorbeelden hoe je dit kan doen. Deze staan in de directory ExampleFromSheets/Relationships.cs.
    public static async Task<IEnumerable<Brewmaster>> GetAllBrouwmeestersIncludesAddress()
        => await DbHelper.GetConnection().QueryAsync<Brewmaster, Address, Brewmaster>(@"
SELECT 
    Brewmaster.*,
    Address.*
FROM
    Brewmaster Brewmaster
JOIN Address Address ON Address.AddressId = Brewmaster.AddressId
ORDER BY Brewmaster.Name
        ",
        map: (result, obj) => {
            result.Address = obj;
            return result;
        },
        splitOn: "AddressId");

    // 3.2 Question
    // 1 op 1 relatie (one-to-one relationship)
    // Haal alle brouwmeesters op en zorg ervoor dat de brouwer (Brewer) gevuld is.
    // Sorteer op naam.
    public static async Task<IEnumerable<Brewmaster>> GetAllBrewmastersWithBrewery()
        => await DbHelper.GetConnection().QueryAsync<Brewmaster, Brewer, Brewmaster>(@"
SELECT 
    Brewmaster.*,
    Brewer.*
FROM
    Brewmaster Brewmaster
JOIN Brewer Brewer ON Brewer.BrewerId = Brewmaster.BrewerId
ORDER BY Brewmaster.Name
        ",
        map: (result, obj) => {
            result.Brewer = obj;
            return result;
        },
        splitOn: "BrewerId");

    // 3.3 Question
    // 1 op 1 (0..1) (one-to-one relationship) 
    // Geef alle brouwers op en zorg ervoor dat de brouwmeester gevuld is.
    // Sorteer op brouwernaam.
    //
    // Niet alle brouwers hebben een brouwmeester.
    // Let op: gebruik het correcte type JOIN (JOIN, LEFT JOIN, RIGHT JOIN).
    // Dapper snapt niet dat het om een 1 - 0..1 relatie gaat.
    // De Query methode ziet er als volgt uit (let op het vraagteken optioneel):
    // Query<Brewer, Brewmaster?, Brewer>(sql, map: ...)
    // Wat je kan doen is in de map functie een controle toevoegen:
    // if (brewmaster is not null) { brewer.Brewmaster = brewmaster; }
    public static async Task<IEnumerable<Brewer>> GetAllBrewersIncludeBrewmaster()
        => await DbHelper.GetConnection().QueryAsync<Brewer, Brewmaster?, Brewer>(@"
SELECT 
    Brewer.*,
    Brewmaster.*
FROM
    Brewer Brewer
LEFT JOIN  Brewmaster Brewmaster ON Brewer.BrewerId = Brewmaster.BrewerId
ORDER BY Brewer.Name
        ",
        map: (result, obj) => {
            if (obj is not null)
                result.Brewmaster = obj;
            return result;
        },
        splitOn: "BrewmasterId");
    
    // 3.4 Question
    // 1 op veel relatie (one-to-many relationship)
    // Geef een overzicht van alle bieren. Zorg ervoor dat de property Brewer gevuld is.
    // Sorteer op biernaam.
    // Zorg ervoor dat bieren van dezelfde brouwerij naar dezelfde instantie van Brouwer verwijzen.
    // Dit kan je doen door een Dictionary<int, Brouwer> te gebruiken.
    // Kijk in voorbeelden hoe je dit kan doen. Deze staan in de directory ExampleFromSheets/Relationships.cs.
    public async static Task<IEnumerable<Beer>> GetAllBeersIncludeBrewery()
    {
        var dict = new Dictionary<int, Brewer>();
        using var conn = DbHelper.GetConnection();

        return await conn.QueryAsync<Beer, Brewer, Beer>(@"
SELECT
    Beer.*,
    Brewer.*
FROM
    Beer Beer
LEFT JOIN Brewer Brewer ON Brewer.BrewerId = Beer.BrewerId
ORDER BY Beer.Name
        ",
        map: (result, obj) => {
            result.Brewer = dict.GetOrAdd(obj.BrewerId, obj);
            return result;
        },
        splitOn: "BrewerId");
    }
    
    // 3.5 Question
    // N+1 probleem (1-to-many relationship)
    // Geef een overzicht van alle brouwerijen en hun bieren. Sorteer op brouwerijnaam en daarna op biernaam.
    // Doe dit door eerst een Query<Brewer>(...) te doen die alle brouwerijen ophaalt. (Dit is 1)
    // Loop (foreach) daarna door de brouwerijen en doe voor elke brouwerij een Query<Beer>(...)
    // die de bieren ophaalt voor die brouwerij. (Dit is N)
    // Dit is een N+1 probleem. Hoe los je dit op? Dat zien we in de volgende vragen.
    // Als N groot is (veel brouwerijen) dan kan dit een performance probleem zijn of worden. Probeer dit te voorkomen!
    public static async Task<IEnumerable<Brewer>> GetAllBrewersIncludingBeersNPlus1()
        => await GetAllBrewersIncludeBeers();
    
    // 3.6 Question
    // 1 op n relatie (one-to-many relationship)
    // Schrijf een query die een overzicht geeft van alle brouwerijen. Vul per brouwerij de property Beers (List<Beer>) met de bieren van die brouwerij.
    // Sorteer op brouwerijnaam en daarna op biernaam.
    // Gebruik de methode Query<Brewer, Beer, Brewer>(sql, map: ...)
    // Het is belangrijk dat je de map functie gebruikt om de bieren te vullen.
    // De query geeft per brouwerij meerdere bieren terug. Dit is een 1 op veel relatie.
    // Om ervoor te zorgen dat de bieren van dezelfde brouwerij naar dezelfde instantie van Brewer verwijzen,
    // moet je een Dictionary<int, Brewer> gebruiken.
    // Dit is een veel voorkomend patroon in Dapper.
    // Vergeet de Distinct() methode te gebruiken om dubbel brouwerijen (Brewer) te voorkomen.
    //  Query<...>(...).Distinct().ToList().
    
    public static async Task<IEnumerable<Brewer>> GetAllBrewersIncludeBeers()
    {
        var dict = new Dictionary<int, Brewer>();
        using var conn = DbHelper.GetConnection();
        
        await conn.QueryAsync<Brewer, Beer, Brewer>(@"
SELECT
    Brewer.*,
    Beer.*
FROM
    Brewer Brewer
LEFT JOIN Beer Beer ON Brewer.BrewerId = Beer.BrewerId
ORDER BY Brewer.Name, Beer.Name
        ",
        map: (result, obj) => {
            result = dict.GetOrAdd(obj.BrewerId, result);
            result.Beers.Add(obj);
            return result;
        },
        splitOn: "BeerId");

        return dict.Values;
    }
    
    // 3.7 Question
    // Optioneel:
    // Dezelfde vraag als hiervoor, echter kan je nu ook de Beers property van Brewer vullen met de bieren?
    // Hiervoor moet je wat extra logica in map methode schrijven.
    // Let op dat er geen dubbelingen komen in de Beers property van Beer!
    public static async Task<IEnumerable<Beer>> GetAllBeersIncludeBreweryAndIncludeBeersInBrewery()
    {
        using var conn = DbHelper.GetConnection();

        var brewers = new Dictionary<int, Brewer>();
        return await DbHelper.GetConnection().QueryAsync<Beer, Brewer, Beer>(@"
SELECT
    Beer.*,
    Brewer.*
FROM
    Beer Beer
LEFT JOIN Brewer Brewer ON Brewer.BrewerId = Beer.BrewerId
ORDER BY Beer.Name
        ",
        map: (result, obj) => {
            var brewer = brewers.GetOrAdd(obj.BrewerId, obj);
            brewer.Beers.Add(result);
            result.Brewer = brewer;
            return result;
        },
        splitOn: "BrewerId");
    }
    
    // 3.8 Question
    // n op n relatie (many-to-many relationship)
    // Geef een overzicht van alle cafés en welke bieren ze schenken.
    // Let op een café kan meerdere bieren schenken. En een bier wordt vaak in meerdere cafe's geschonken. Dit is een n op n relatie.
    // Sommige cafés schenken geen bier. Dus gebruik LEFT JOINS in je query.
    // Bij n op n relaties is er altijd spraken van een tussen-tabel (JOIN-table, associate-table), in dit geval is dat de tabel Sells.
    // Gebruikt de multi-mapper Query<Cafe, Beer, Cafe>("query", splitOn: "splitCol1, splitCol2").
    // Gebruik de klassen Cafe en Beer.
    // De bieren worden opgeslagen in een de property Beers (List<Beer>) van de klasse Cafe.
    // Sorteer op cafénaam en daarna op biernaam.
    
    // Kan je ook uitleggen wat het verschil is tussen de verschillende JOIN's en wat voor gevolg dit heeft voor het resultaat?
    // Het is belangrijk om te weten wat de verschillen zijn tussen de verschillende JOIN's!!!! Als je dit niet weet, zoek het op!
    // Als je dit namelijk verkeerd doet, kan dit grote gevolgen hebben voor je resultaat (je krijgt dan misschien een verkeerde aantal records).
    public async static Task<IEnumerable<Cafe>> GetAllBeersServedPerCafe()
    {
        using var conn = DbHelper.GetConnection();
        var cafes = new Dictionary<int, Cafe>();
        _ = await DbHelper.GetConnection().QueryAsync<Cafe, Beer, Cafe>(@"
SELECT
	Cafe.*,
    Beer.*
FROM
    Cafe Cafe
LEFT JOIN Sells Sells ON Sells.CafeId = Cafe.CafeId
LEFT JOIN Beer Beer ON Sells.BeerId = Beer.BeerId
ORDER BY Cafe.Name, Beer.Name;
        ",
        map: (result, obj) => {
            result = cafes.GetOrAdd(result.CafeId, result);
            result.Beers.Add(obj);
            return result;
        },
        splitOn: "BeerId");

        return cafes.Values;
    }

    // 3.9 Question
    // We gaan nu nog een niveau dieper. Geef een overzicht van alle brouwerijen, met daarin de bieren die ze verkopen,
    // met daarin in welke cafés ze verkocht worden.
    // Sorteer op brouwerijnaam, biernaam en cafenaam. 
    // Gebruik (vul) de class Brewer, Beer en Cafe.
    // Gebruik de methode Query<Brewer, Beer, Cafe, Brewer>(...) met daarin de juiste JOIN's in de query en splitOn parameter.
    // Je zult twee dictionaries moeten gebruiken. Een voor de brouwerijen en een voor de bieren.
    public async static Task<IEnumerable<Brewer>> GetAllBrewersIncludeBeersThenIncludeCafes()
    {
        using var conn = DbHelper.GetConnection();
        var cafes = new Dictionary<int, Cafe>();
        var brewers = new Dictionary<int, Brewer>();
        var beers = new Dictionary<int, Beer>();

        // This entire thing could be replaced by SQL Server returning a
        // JSON statement with ever bit of data and then deserializing...
        // This is the kind of thing that should be done in the database...
        _ = await DbHelper.GetConnection().QueryAsync<Brewer, Beer, Cafe, Brewer>(@"
SELECT
    Brewer.*,
    Beer.*,
	Cafe.*
FROM
    Brewer Brewer
LEFT JOIN Beer Beer ON Beer.BrewerId = Brewer.BrewerId
LEFT JOIN Sells Sells ON Sells.BeerId = Beer.BeerId
LEFT JOIN Cafe Cafe ON Sells.CafeId = Cafe.CafeId
ORDER BY Brewer.Name, Beer.Name, Cafe.Name;
        ",
        map: (brewer, beer, cafe) =>
        {
            brewer = brewers.GetOrAdd(brewer.BrewerId, brewer);
            beer = beers.GetOrAdd(beer.BeerId, beer);

            if(cafe is not null)
                beer.Cafes.Add(cafes.GetOrAdd(cafe.CafeId, cafe));

            brewer.Beers.Add(beer);

            return brewer;
        },
        splitOn: "BeerId, CafeId");

        return brewers.Values;
    }
    
    // 3.10 Question - Er is WEL een test voor deze vraag
    // Optioneel: Geef een overzicht van alle bieren en hun de bijbehorende brouwerij.
    // Sorteer op brouwerijnaam, biernaam.
    // Gebruik hiervoor een View BeerAndBrewer (maak deze zelf). Deze view bevat alle informatie die je nodig hebt gebruikt join om de tabellen Beer, Brewer.
    // Let op de kolomnamen in de view moeten uniek zijn. Dit moet je dan herstellen in de query waarin je view gebruik zodat Dapper het snap
    // (SELECT BeerId, BeerName as Name, Type, ...). Zie BeerName als voorbeeld hiervan.
    public async static Task<IEnumerable<Beer>> GetBeerAndBrewersByView()
    {
        using var conn = DbHelper.GetConnection();
        var brewers = new Dictionary<int, Brewer>();

        _ = await conn.ExecuteAsync(@"
CREATE OR REPLACE VIEW vw_BeerBrewer AS (
    SELECT
        Beer.BeerId,
        Beer.Name AS BeerName,
        Beer.Type,
        Beer.Style,
        Beer.Alcohol,
        Brewer.Name AS BrewerName,
        Brewer.BrewerId,
        Brewer.Country
    FROM
        Beer Beer
    LEFT JOIN Brewer Brewer ON Brewer.BrewerId = Beer.BrewerId
);");

        return await conn.QueryAsync<vw_BeerBrewer, Brewer, Beer>(@"
SELECT * FROM vw_BeerBrewer ORDER BY BrewerName, BeerName
        ",
        map: (result, obj) => {
            obj.Name = result.BrewerName;

            return new Beer {
                BrewerId = obj.BrewerId,
                BeerId = result.BeerId,
                Name = result.BeerName,
                Style = result.Style,
                Type = result.Type,
                Alcohol = result.Alcohol,
                Brewer = brewers.GetOrAdd(obj.BrewerId, obj),
            };
        },
        splitOn: "BrewerId");
    }

    private class vw_BeerBrewer : Model.Beer
    {
        public required string BeerName { get; set; }
        public required string BrewerName { get; set; }
    }
}