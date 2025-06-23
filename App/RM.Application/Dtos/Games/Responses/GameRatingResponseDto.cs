namespace App.Dtos.Games.Responses;

public class GameRatingResponseDto
{
    public required Guid GameId { get; init; }
    public required string Slug { get; init; }
    public required int Rating { get; init; }
}