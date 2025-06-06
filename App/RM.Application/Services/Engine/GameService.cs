using App.Interfaces.Engine;
using RM.Domain.Entities.Games;

namespace App.Services.Engine;

public class GameService(IGamesRepository gamesRepository, IGameService gameService) : IGameService
{
    public Task<bool> CreateAsync(Game game, CancellationToken cancellationToken = default)
    {
        return gamesRepository.CreateAsync(game, cancellationToken);
    }

    public Task<Game?> GetByIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        return gamesRepository.GetByIdAsync(gameId, cancellationToken);
    }

    public Task<Game?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return gamesRepository.GetBySlugAsync(slug, cancellationToken);
    }

    public async Task<Game?> UpdateAsync(Game game, CancellationToken cancellationToken = default)
    {
        var movieExists = await gamesRepository.ExistsByIdAsync(game.GameId, cancellationToken);

        if (!movieExists) return null;
        
        await gamesRepository.UpdateAsync(game, cancellationToken);
        return game;
    }

    public Task<bool> DeleteByIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        return gamesRepository.DeleteByIdAsync(gameId, cancellationToken);
    }

    public Task<IEnumerable<Game>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return gamesRepository.GetAllAsync(cancellationToken);
    }
}