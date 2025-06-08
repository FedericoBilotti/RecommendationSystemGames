namespace App.Interfaces.Engine;

public interface IRatingService
{
    Task<bool> RateGameAsync(Guid gameId, int rating, Guid userId, CancellationToken cancellationToken = default);
}