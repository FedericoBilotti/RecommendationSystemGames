using App.Dtos.Games.Requests;
using App.Dtos.Games.Responses;
using App.Interfaces.Engine;
using App.Mappers;
using FluentValidation;
using RM.Domain.Entities.Games;

namespace App.UseCases.Engine;

public class GameUseCase(IGamesRepository gamesRepository, IRatingRepository ratingRepository, IValidator<Game> gameValidatorService) : IGameUseCase
{
    public async Task<GameResponseDto?> CreateAsync(CreateGameRequestDto createGameRequestDto, CancellationToken cancellationToken = default)
    {
        Game game = createGameRequestDto.MapToGame();
        await gameValidatorService.ValidateAndThrowAsync(game, cancellationToken);
        
        bool gameWasCreated = await gamesRepository.CreateAsync(game, cancellationToken);

        return gameWasCreated ? game.MapToResponse() : null;
    }

    public Task<Game?> GetByIdAsync(Guid gameId, Guid? userId = default, CancellationToken cancellationToken = default)
    {
        return gamesRepository.GetByIdAsync(gameId, userId, cancellationToken);
    }

    public Task<Game?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken cancellationToken = default)
    {
        return gamesRepository.GetBySlugAsync(slug, userId, cancellationToken);
    }

    public async Task<GameResponseDto?> UpdateAsync(UpdateGameRequestDto gameDto, Guid gameId, Guid? userId = default, CancellationToken cancellationToken = default)
    {
        Game game = gameDto.MapToGame(gameId);
        await gameValidatorService.ValidateAndThrowAsync(game, cancellationToken);
        
        bool movieExists = await gamesRepository.ExistsByIdAsync(game.GameId, cancellationToken);

        if (!movieExists)
        {
            return null;
        }
        
        await gamesRepository.UpdateAsync(game, cancellationToken);

        if (!userId.HasValue)
        {
            float? rating = await ratingRepository.GetRatingAsync(game.GameId, cancellationToken);
            game.Rating = rating;
            return game.MapToResponse();
        }

        (float? Rating, int? User) ratings = await ratingRepository.GetUserRatingAsync(game.GameId, userId.Value, cancellationToken);
        game.Rating = ratings.Rating;
        game.UserRating = ratings.User;
        return game.MapToResponse();
    }

    public Task<bool> DeleteByIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        return gamesRepository.DeleteByIdAsync(gameId, cancellationToken);
    }

    public Task<IEnumerable<Game>> GetAllAsync(Guid? userId = default, CancellationToken cancellationToken = default)
    {
        return gamesRepository.GetAllAsync(userId, cancellationToken);
    }
}