using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace RM.Domain.Entities.Games;

public class Game
{
    [Key] public Guid GameId { get; init; }
    public string Title { get; init; }
    public string slug => GenerateSlug();
    public required string Description { get; init; }
    public required string YearOfRelease { get; init; }

    public int UserRating { get; init; } 
    
    [ForeignKey("GenreId")]
    public List<Genre> Genres { get; init; } = [];
    
    private string GenerateSlug()
    {
        string slugTitle = Regex.Replace(Title, "[^0-9A-Za-z _-]", string.Empty)
                .ToLower().Replace(" ", "-");
        return $"{slugTitle}-{YearOfRelease}";
    }
}