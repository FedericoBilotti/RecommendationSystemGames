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
            GameId = Guid.NewGuid(),
            Title = createGameRequestDto.Title,
            Description = createGameRequestDto.Description,
            YearOfRelease = createGameRequestDto.YearOfRelease,
            Genres = createGameRequestDto.Genre.ToList()
        };
    }

    public static Game MapToGame(this UpdateGameRequestDto updateGameRequestDto, Guid id)
    {
        return new Game
        {
            GameId = id,
            Title = updateGameRequestDto.Title,
            Description = updateGameRequestDto.Description,
            YearOfRelease = updateGameRequestDto.YearOfRelease,
            Genres = updateGameRequestDto.Genre.ToList()
        };
    }

    public static GameResponseDto MapToResponse(this Game game)
    {
        var gameResponse = new GameResponseDto
        {
            GameId = game.GameId,
            Title = game.Title,
            Description = game.Description,
            Slug = game.Slug,
            YearOfRelease = game.YearOfRelease,
            Genre = game.Genres
        };
        
        if (game.UserRating != 0)
            gameResponse.UserRating = game.UserRating;
        
        return gameResponse;
    }

    public static GamesResponseDto MapToResponse(this IEnumerable<Game> game)
    {
        return new GamesResponseDto
        {
            GamesResponseDtos = game.Select(g => g.MapToResponse()).ToList()
        };
    }
}