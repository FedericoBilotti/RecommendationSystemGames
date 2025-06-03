namespace App.Dtos.Games.Responses;

public class GamesResponseDto
{
    public required IEnumerable<GameResponseDto> Games { get; init; } = [];
}