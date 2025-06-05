using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace RM.Domain.Entities.Games;

public class Game
{
    public Guid GameId { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string YearOfRelease { get; init; }
    public int UserRating { get; init; }
    public required List<Genre> Genres { get; init; } = new();

    public string Slug => GenerateSlug();

    private string GenerateSlug()
    {
        string slugTitle = Regex.Replace(Title, "[^0-9A-Za-z _-]", string.Empty).ToLower().Replace(" ", "-");
        return $"{slugTitle}-{YearOfRelease}";
    }
}