using App.Interfaces.Engine;
using App.Validators;
using FluentValidation;
using RM.Domain.Entities.Games;

namespace App.Services.Engine;

public class GameService(IGamesRepository gamesRepository, IValidator<Game> gameValidator) : IGameService
{
    public async Task<bool> CreateAsync(Game game, CancellationToken cancellationToken = default)
    {
        await gameValidator.ValidateAndThrowAsync(game, cancellationToken);
        
        return await gamesRepository.CreateAsync(game, cancellationToken);
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
        await gameValidator.ValidateAndThrowAsync(game, cancellationToken);
        
        bool movieExists = await gamesRepository.ExistsByIdAsync(game.GameId, cancellationToken);

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