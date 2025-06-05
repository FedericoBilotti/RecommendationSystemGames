namespace App.Dtos.Games.Responses;

public class GameResponseDto
{
    public required Guid GameId { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required int YearOfRelease { get; init; }
    public int UserRating { get; init; }
    public required IEnumerable<string> Genre { get; init; } = Enumerable.Empty<string>();
}