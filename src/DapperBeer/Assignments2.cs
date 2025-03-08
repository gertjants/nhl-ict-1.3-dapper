using System.Data;
using Dapper;
using DapperBeer.DTO;
using DapperBeer.Model;
using FluentAssertions;
using SqlKata;

namespace DapperBeer;

public class Assignments2
{
    // 2.1 Question
    // !!!Doe dit nooit!!!!!
    // Wees voorzichtig en doe dit in de praktijk nooit!!!!!!
    // We gaan SQL-injectie gebruiken om te laten zien hoe het werkt.
    // Maak de methode die alle bieren teruggeeft, gegeven het land.
    // Normaal gesproken zouden we hiervoor een WHERE Country = @Country gebruiken in de sql.
    // Dit is veilig omdat er een query parameter placeholder wordt gebruikt.
    // Echter, we gaan nu het volgende doen:
    //  string sql = $"SELECT Name FROM ... WHERE Country = '{country}'";
    // Dit is onveilig en zal SQL-injectie veroorzaken!
    // Hetzelfde geldt overigens ook voor:
    //  string sql = "SELECT Name FROM ... WHERE Country = '" + country + "'";
    // Kijk goed naar de test, het tweede deel van de test doet een SQL-injectie en we krijgen dus toch alle bieren terug.
    // Ook al zou normaal gesproken dit niet de intentie zijn.
    // Dit is een voorbeeld van een SQL-injectie, dit geval is niet zo gevaarlijk, maar het kan veel gevaarlijker zijn
    // omdat we op de plek waar de SQL-injectie plaatsvindt, alles kunnen doen wat we willen
    // (DELETE, DROP, SELECT van andere tabellen, etc.)!
    // Met andere woorden gebruik altijd query parameters en nooit string concatenatie om SQL-queries te maken.
    // !!!DOE DIT NOOIT MEER SVP!!!!
    public async static Task<IEnumerable<string>> GetBeersByCountryWithSqlInjection(string country)
        => await DbHelper.GetConnection().QueryAsync<string>(@$"
SELECT 
    beer.Name 
FROM Beer beer
JOIN Brewer brewer ON beer.BrewerId = brewer.BrewerId 
WHERE brewer.Country='{country}'");
    
    // 2.2 Question
    // Maak een methode die alle bieren teruggeeft, gegeven het land, echter het land kan ook leeg gelaten worden.
    // Sorteer de bieren op naam.
    // Als het land leeg is, dan moeten alle bieren teruggegeven worden.
    // Hiervoor kan je NULL-truc gebruiken in je SQL (zie de sheets).
    //      WHERE @Country IS NULL OR Country = @Country 
    // Het vraagteken bij 'GetAllBeersByCountry(string? country)' geeft aan dat het argument optioneel is (string? country).
    // Dit betekent dus dat country null kan zijn.
    public async static Task<IEnumerable<string>> GetAllBeersByCountry(string? country)
        => await GetAllBeersByCountryAndMinAlcohol(country);
    
    // 2.3 Question
    // Nu doen we hetzelfde als in de vorige opdracht GetAllBeersByCountry, echter voegen we een extra parameter toe,
    // het minimal alcoholpercentage.
    // Ook het minAlcohol kan leeg gelaten worden (decimal? minAlcohol).
    // Gebruikt >= (groter of gelijk aan) voor de vergelijking van het minAlcohol.
    public async static Task<IEnumerable<string>> GetAllBeersByCountryAndMinAlcohol(string? country = null, decimal? minAlcohol = null)
        => await DbHelper.GetConnection().QueryAsync<string>(@$"
SELECT 
    beer.Name 
FROM Beer beer
JOIN Brewer brewer ON beer.BrewerId = brewer.BrewerId 
WHERE 
    (@Country IS NULL OR brewer.Country=@Country)
    AND (@MinAlcohol IS NULL OR beer.Alcohol >= @MinAlcohol)
ORDER BY beer.Name
    ", new { 
        Country = country,
        MinAlcohol = minAlcohol
    });
    
    // 2.4 Question
    // Helaas kan je in SQL bijv. geen parameter gebruiken voor de ORDER BY.
    // Dit kan je oplossen door de SQL te bouwen met een StringBuilder of een SqlBuilder.
    // De SqlBuilder is een handige tool om SQL-queries te bouwen.
    //  Voor uitleg zie: https://github.com/DapperLib/Dapper/blob/main/Dapper.SqlBuilder/Readme.md
    
    // Maak onderstaande methode die bieren teruggeeft (gebruik onderstaande query).
    // SELECT beer.Name
    // FROM Beer beer 
    //     JOIN Brewer brewer ON beer.BrewerId = brewer.BrewerId 
    //     WHERE (@Country IS NULL OR Country = @Country) AND (@MinAlcohol IS NULL OR Alcohol >= @MinAlcohol)
    // ORDER BY beer.Name
    //
    // Echter, vervang de WHERE-clausule door gebruik te maken van de SqlBuilder.
    // 
    // Je krijgt dan dergelijke constructies in je code:
    // if(country != null) {
    //     builder = builder.Where("brewer.Country = @Country", new { Country = country });
    // }
    // De queryBuilder maakt het bijvoorbeeld mogelijk om een WHERE-clausule te bouwen met meerdere voorwaarden.
    // Dit kan handig voor het maken van een datagrid (tabel) waarbij de gebruiker meerdere filters / sorteringen kan gebruiken.
    
    // Voeg zelf code toe om minAlcohol te kunnen gebruiken in je WHERE clausule.
    // 
    // Voeg ook een ORDER BY toe, zodat de bieren op verschillende kolommen gesorteerd kunnen worden.
    // Hiervoor is de parameter (string orderBy = "beer.Name") toegevoegd aan de methode. 
    // Deze parameter heeft een default waarde. In de test kan je zien hoe deze methode aangeroepen wordt
    // (en dan met name naar de parameters).
    // Dit worden default parameters genoemd i.c.m. optionele parameters en named arguments. 
    // Om Dapper (en andere bibliotheken) te gebruiken, worden vaak bovenstaande technieken gebruikt voor het aanroepen van methoden.
    // Zo zie je maar dat er ook wat van je C#-skills verwacht wordt :-).
    public async static Task<IEnumerable<string>> GetAllBeersByCountryAndMinAlcoholOrderByWithSqlBuilder(
        string? country = null, decimal? minAlcohol = null, string orderBy = "beer.Name")
    {
        var builder = new SqlBuilder();
        var selectTemplate = builder.AddTemplate(@$"
SELECT beer.Name
FROM Beer beer 
    JOIN Brewer brewer ON beer.BrewerId = brewer.BrewerId 
/**where**/
/**orderby**/");

        if(!string.IsNullOrWhiteSpace(country))
            builder.Where($"brewer.Country = @{nameof(country)}", new { country });

        if(minAlcohol is not null)
            builder.Where($"beer.Alcohol >= @{nameof(minAlcohol)}", new { minAlcohol });

        if(!string.IsNullOrWhiteSpace(orderBy))
            builder.OrderBy(orderBy);
            
        return await DbHelper.GetConnection().QueryAsync<string>(selectTemplate.RawSql, selectTemplate.Parameters);
    }

    // 2.5 Question
    // Maak een view die de naam van het bier teruggeeft en de naam van de brouwerij
    // en de naam van de brouwmeester als de brouwerij deze heeft (LEFT JOIN).
    // Sorteer de resultaten op bier naam.
    //
    // Gebruik de volgende where-clause: WHERE BrewmasterName IS NOT NULL (in de query niet in de view)
    // Gebruik de klasse BrewerBeerBrewmaster om de resultaten in op te slaan. (directory DTO).
    public async static Task<IEnumerable<BrewerBeerBrewmaster>> GetAllBeerNamesWithBreweryAndBrewmaster()
    {
        using var conn = DbHelper.GetConnection();

        await conn.ExecuteAsync(@"
CREATE OR REPLACE VIEW vw_BrewerBeerBrewmaster AS (
    SELECT 
        Beer.Name AS BeerName,
        Brewer.Name AS BrewerName,
        Brewmaster.Name AS BrewmasterName
    FROM
        Beer Beer
    JOIN Brewer Brewer ON Brewer.BrewerId = Beer.BrewerId
    LEFT JOIN Brewmaster Brewmaster ON Brewmaster.BrewerId = Brewer.BrewerId
);");
    
        return await conn.QueryAsync<BrewerBeerBrewmaster>(@"
SELECT 
    *
FROM 
    vw_BrewerBeerBrewmaster
WHERE 
    BrewmasterName IS NOT NULL
ORDER BY 
    BeerName");
    }
    
    // 2.6 Question
    // Soms is het onhandig om veel parameters mee te geven aan een methode.
    // Dit kan je oplossen door een klasse te maken die de parameters bevat.
    // De kan je rechtstreeks meegeven aan de Query<T>(sql, param: filter).
    // of aan SqlBuilder.Template? queryTemplate = builder.AddTemplate(sql, filter);
    // Maak een query die bieren teruggeeft, gegeven het land en het type (beide optioneel).
    // Ook willen we een limiet en een offset meegeven in de query.
    // LIMIT @PageSize OFFSET @Offset  
    // Sorteer op OrderBy
    // Zie de klasse BeerFilter.
    public class BeerFilter
    {
        public string? Country { get; set; }
        public string? Type { get; set; }
        public int PageSize { get; set; } = 10;    //default value start at 0
        public int PageIndex { get; set; } = 0;    //default value start at 0
        
        public int Offset => PageSize * (PageIndex+1);
        
        public string OrderBy { get; set; } = "beer.Name";
    }
    public async static Task<IEnumerable<Beer>> GetBeersByCountryAndType(BeerFilter filter)
    {        
        SqlBuilder builder = new SqlBuilder();

        var selectTemplate = builder.AddTemplate(@$"
SELECT beer.BeerId, beer.Name, beer.Type, beer.Style, beer.Alcohol, beer.BrewerId
FROM Beer beer 
    JOIN Brewer brewer ON beer.BrewerId = brewer.BrewerId
/**where**/
/**orderby**/
LIMIT @PageSize OFFSET @Offset");

        if(!string.IsNullOrWhiteSpace(filter.Country))
            builder.Where($"brewer.Country = @Country");

        if(!string.IsNullOrWhiteSpace(filter.Type))
            builder.Where($"beer.Type = @Type");

        if(!string.IsNullOrWhiteSpace(filter.OrderBy))
            builder.OrderBy(filter.OrderBy);

        return await DbHelper.GetConnection().QueryAsync<Beer>(selectTemplate.RawSql, filter);
    }
}