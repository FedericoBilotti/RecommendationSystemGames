namespace RM.Domain.Entities.Games;

public class GameRating
{
    public required Guid GameId { get; init; }
    public required int Slug { get; init; }
    public required int Rating { get; init; }
}