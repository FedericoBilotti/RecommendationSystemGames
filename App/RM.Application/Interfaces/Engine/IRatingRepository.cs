namespace App.Interfaces.Engine;

public interface IRatingRepository
{
    Task<float?> GetRatingAsync(Guid gameId, CancellationToken cancellationToken = default);
    Task<(float? Rating, int? User)> GetUserRatingAsync(Guid gameId, Guid userId, CancellationToken cancellationToken = default);
}