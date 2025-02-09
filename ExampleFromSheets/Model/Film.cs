namespace ExampleFromSheets.Model;

public class Film
{
    public int FilmId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int ReleaseYear { get; set; }
    public int LanguageId { get; set; }
    public int OriginalLanguageId { get; set; }
    public int RentalDuration { get; set; }
    public decimal RentalRate { get; set; }
    public int Length { get; set; }
    public decimal ReplacementCost { get; set; }
    public string Rating { get; set; }
    public string SpecialFeatures { get; set; }
    public DateTime LastUpdate { get; set; }

    public List<Actor> Actors { get; set; } = new List<Actor>();
}