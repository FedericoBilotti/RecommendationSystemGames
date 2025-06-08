using App.Interfaces.Engine;
using FluentValidation;
using FluentValidation.Results;

namespace App.Services.Engine;

public class RatingService(IRatingRepository ratingRepository, IGamesRepository gamesRepository) : IRatingService
{
    public async Task<bool> RateGameAsync(Guid gameId, int rating, Guid userId, CancellationToken cancellationToken = default)
    {
        if (rating is < 0 or > 5)
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
}
