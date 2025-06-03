using App.Dtos.Games.Requests;
using App.Dtos.Games.Responses;
using RM.Domain.Entities.Games;

namespace RM.Presentation.Mappers;

public static class GameMapping
{
    public static Game MapToGame(this CreateGameRequestDto createGameRequestDto)
    {
        return new Game
        {
            Title = createGameRequestDto.Title,
            Description = createGameRequestDto.Description,
            YearOfRelease = createGameRequestDto.YearOfRelease,
            Genres = createGameRequestDto.Genre.Select(g => new Genre { Name = g }).ToList()
        };
    }

    public static GameResponseDto MapToGameResponseDto(this Game game)
    {
        return new GameResponseDto
        {
            GameId = game.GameId,
            Title = game.Title,
            Description = game.Description,
            YearOfRelease = game.YearOfRelease,
            Genre = game.Genres.Select(g => g.Name).ToList()
        };
    }
}