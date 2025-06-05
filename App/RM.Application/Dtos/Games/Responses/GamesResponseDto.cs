namespace App.Dtos.Games.Responses;

public class GamesResponseDto
{
    public required IEnumerable<GameResponseDto> GamesResponseDtos { get; init; } = Enumerable.Empty<GameResponseDto>();
}