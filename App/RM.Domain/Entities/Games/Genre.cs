using System.ComponentModel.DataAnnotations;

namespace RM.Domain.Entities.Games;

public class Genre
{
    [Key] public Guid GenreId { get; init; }
    public string Name { get; init; } = string.Empty;
}