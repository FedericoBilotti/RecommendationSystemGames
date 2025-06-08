using App.Interfaces.Engine;
using FluentValidation;
using RM.Domain.Entities.Games;

namespace App.Services.Engine;

public class GameService(IGamesRepository gamesRepository, IRatingRepository ratingRepository, IValidator<Game> gameValidator) : IGameService
{
    public async Task<bool> CreateAsync(Game game, CancellationToken cancellationToken = default)
    {
        await gameValidator.ValidateAndThrowAsync(game, cancellationToken);
        
        return await gamesRepository.CreateAsync(game, cancellationToken);
    }

    public Task<Game?> GetByIdAsync(Guid gameId, Guid? userId = default, CancellationToken cancellationToken = default)
    {
        return gamesRepository.GetByIdAsync(gameId, userId, cancellationToken);
    }

    public Task<Game?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken cancellationToken = default)
    {
        return gamesRepository.GetBySlugAsync(slug, userId, cancellationToken);
    }

    public async Task<Game?> UpdateAsync(Game game, Guid? userId = default, CancellationToken cancellationToken = default)
    {
        await gameValidator.ValidateAndThrowAsync(game, cancellationToken);
        
        bool movieExists = await gamesRepository.ExistsByIdAsync(game.GameId, cancellationToken);

        if (!movieExists) return null;
        
        await gamesRepository.UpdateAsync(game, cancellationToken);

        if (!userId.HasValue)
        {
            float? rating = await ratingRepository.GetRatingAsync(game.GameId, cancellationToken);
            game.Rating = rating;
            return game;
        }

        (float? Rating, int? User) ratings = await ratingRepository.GetUserRatingAsync(game.GameId, userId.Value, cancellationToken);
        game.Rating = ratings.Rating;
        game.UserRating = ratings.User;
        return game;
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