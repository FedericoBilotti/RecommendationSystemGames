using App.Dtos.Games.Responses;

namespace App.Interfaces.Engine;

public interface IRatingUseCase
{
    Task<bool> RateGameAsync(Guid gameId, int rating, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteRatingAsync(Guid gameId, Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<GameRatingResponseDto>> GetUserRatingsAsync(Guid userId, CancellationToken cancellationToken = default);
}