using RM.Domain.Entities.Games;

namespace App.Interfaces.Engine;

public interface IGameService
{
    Task<bool> CreateAsync(Game game, CancellationToken cancellationToken = default);
    Task<Game?> GetByIdAsync(Guid gameId, Guid? userId = default, CancellationToken cancellationToken = default);
    Task<Game?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken cancellationToken = default);
    Task<Game?> UpdateAsync(Game game, Guid? userId = default, CancellationToken cancellationToken = default);
    Task<bool> DeleteByIdAsync(Guid gameId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Game>> GetAllAsync(Guid? userId = default, CancellationToken cancellationToken = default);
}