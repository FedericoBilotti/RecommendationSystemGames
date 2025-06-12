using App.Dtos.Games.Requests;
using App.Dtos.Games.Responses;
using RM.Domain.Entities;
using RM.Domain.Entities.Games;

namespace App.Mappers;

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

    public static Game MapToGame(this UpdateGameRequestDto updateGameRequestDto, Guid gameId)
    {
        return new Game
        {
            GameId = gameId,
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
            UserRating = game.UserRating,
            Rating = game.Rating,
            YearOfRelease = game.YearOfRelease,
            Genre = game.Genres
        };
        
        return gameResponse;
    }

    public static GamesResponseDto MapToResponse(this IEnumerable<Game> game, int page, int pageSize, int totalCount)
    {
        return new GamesResponseDto
        {
            GamesResponseDtos = game.Select(g => g.MapToResponse()).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
    
    public static IEnumerable<GameRatingResponseDto> MapToResponse(this IEnumerable<GameRating> ratings)
    {
        return ratings.Select(x => new GameRatingResponseDto
        {
            GameId = x.GameId,
            Slug = x.Slug,
            Rating = x.Rating
        });
    }

    public static GetAllGameOptions MapToOptions(this GetAllGameRequestDto requestDto)
    {
        return new GetAllGameOptions
        {
            Title = requestDto.Title,
            YearOfRelease = requestDto.YearOfRelease,
            SortField = requestDto.SortBy?.TrimStart('+', '-'),
            SortOrder = requestDto.SortBy is null ? SortOrder.Unsorted : 
                    requestDto.SortBy.StartsWith("+") ? SortOrder.Ascending : SortOrder.Descending,
            Page = requestDto.Page,
            PageSize = requestDto.PageSize
        };
    }

    public static GetAllGameOptions WithId(this GetAllGameOptions gameOptions, Guid gameId)
    {
        gameOptions.UserId = gameId;
        return gameOptions;
    }
}