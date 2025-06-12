using RM.Domain.Entities;
using RM.Domain.Entities.Games;

namespace App.Interfaces.Engine;

public interface IGamesRepository
{
    Task<bool> CreateAsync(Game game, CancellationToken cancellationToken = default);
    Task<Game?> GetByIdAsync(Guid gameId, Guid? userId = default, CancellationToken cancellationToken = default);
    Task<Game?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Game game, CancellationToken cancellationToken = default);
    Task<bool> DeleteByIdAsync(Guid gameId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Game>> GetAllAsync(GetAllGameOptions gameOptions, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIdAsync(Guid gameId, CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(string? title = default, int? yearOfRelease = default, CancellationToken cancellationToken = default);
}