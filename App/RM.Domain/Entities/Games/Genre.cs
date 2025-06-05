namespace RM.Domain.Entities.Games;

public class Genre
{
    public Guid GenreId { get; init; }
    public required string Name { get; init; } = string.Empty;
}