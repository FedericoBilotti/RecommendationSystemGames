namespace App.Interfaces.Engine;

public interface IRatingRepository
{
    Task<bool> RateGameAsync(Guid gameId, int rating, Guid userId, CancellationToken cancellationToken = default);
    Task<float?> GetRatingAsync(Guid gameId, CancellationToken cancellationToken = default);
    Task<(float? Rating, int? User)> GetUserRatingAsync(Guid gameId, Guid userId, CancellationToken cancellationToken = default);
}