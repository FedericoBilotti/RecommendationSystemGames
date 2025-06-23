using App.Dtos.Games.Responses;
using App.Interfaces.Engine;
using App.Mappers;
using FluentValidation;
using FluentValidation.Results;
using RM.Domain.Entities.Games;

namespace App.UseCases.Engine;

public class RatingUseCase(IRatingRepository ratingRepository, IGamesRepository gamesRepository) : IRatingUseCase
{
    public async Task<bool> RateGameAsync(Guid gameId, int rating, Guid userId, CancellationToken cancellationToken = default)
    {
        if (rating is < 1 or > 5)
        {
            throw new ValidationException([
                new ValidationFailure
                {
                    PropertyName = "Rating",
                    ErrorMessage = "Rating must be between 1 and 5"
                }
            ]);
        }
        
        bool game = await gamesRepository.ExistsByIdAsync(gameId, cancellationToken);

        if (!game) return false;
        
        return await ratingRepository.RateGameAsync(gameId, rating, userId, cancellationToken);
    }

    public Task<bool> DeleteRatingAsync(Guid gameId, Guid userId, CancellationToken cancellationToken = default)
    {
        return ratingRepository.DeleteRatingAsync(gameId, userId, cancellationToken);
    }

    public async Task<IEnumerable<GameRatingResponseDto>> GetUserRatingsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        IEnumerable<GameRating> ratings = await ratingRepository.GetUserRatingsAsync(userId, cancellationToken);
        return ratings.MapToResponse();
    }
}
