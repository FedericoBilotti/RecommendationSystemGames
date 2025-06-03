using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RM.Domain.Entities.Games;

public class Game
{
    [Key] public Guid GameId { get; init; }
    public int UserRating { get; init; } 
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; } = string.Empty;
    
    [ForeignKey("GenreId")]
    public IEnumerable<Genre> Genres { get; init; } = [];
}