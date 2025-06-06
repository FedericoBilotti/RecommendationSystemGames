using RM.Domain.Entities.Games;

namespace App.Interfaces.Engine;

public interface IGameService
{
    Task<bool> CreateAsync(Game game, CancellationToken cancellationToken = default);
    Task<Game?> GetByIdAsync(Guid gameId, CancellationToken cancellationToken = default);
    Task<Game?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<Game?> UpdateAsync(Game game, CancellationToken cancellationToken = default);
    Task<bool> DeleteByIdAsync(Guid gameId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Game>> GetAllAsync(CancellationToken cancellationToken = default);
}