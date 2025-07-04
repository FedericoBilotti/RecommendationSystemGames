using System.Text.RegularExpressions;

namespace RM.Domain.Entities.Games;

public class Game
{
    public required Guid GameId { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required int YearOfRelease { get; init; }
    
    public float? Rating { get; set; }
    public int? UserRating { get; set; }
    
    public required List<string> Genres { get; init; } = new();

    public string Slug => GenerateSlug();

    private string GenerateSlug()
    {
        string slugTitle = Regex.Replace(Title, "[^0-9A-Za-z _-]", string.Empty).ToLower().Replace(" ", "-");
        return $"{slugTitle}-{YearOfRelease}";
    }
}