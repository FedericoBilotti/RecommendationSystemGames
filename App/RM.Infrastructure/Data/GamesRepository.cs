using App.Interfaces;
using App.Interfaces.Engine;
using Dapper;
using RM.Domain.Entities.Games;

namespace RM.Infrastructure.Data;

public class GamesRepository(IDbConnectionFactory connectionFactory) : IGamesRepository
{
    public async Task<bool> CreateAsync(Game game, CancellationToken cancellationToken = default)
    {        
        using var connection = await connectionFactory.GetConnectionAsync();
        using var transaction = connection.BeginTransaction();

        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                   INSERT INTO games (gameId, title, slug, yearOfRelease, description, userRating)
                                                   VALUES (@gameId, @title, @slug, @yearOfRelease, @description, @userRating)
                                                   """, game));

        if (result > 0)
        {
            foreach (Genre genre in game.Genres)
            {
                await connection.ExecuteAsync(new CommandDefinition("""
                                                                    INSERT INTO genres (gameId, name)
                                                                    VALUES (@gameId, @name)
                                                                    """, new { gameId = game.GameId, name = genre }));
            }
        }

        transaction.Commit();
        return result > 0;
    }

    public Task<Game?> GetByIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Game?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAsync(Game game, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteByIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Game>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsByIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}