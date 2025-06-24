using App.Interfaces;
using App.Interfaces.Engine;
using Dapper;
using RM.Domain.Entities;
using RM.Domain.Entities.Games;

namespace RM.Infrastructure.Data;

public class GamesRepository(IDbConnectionFactory connectionFactory) : IGamesRepository
{
    public async Task<bool> CreateAsync(Game game, CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.GetConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         INSERT INTO games (gameId, title, slug, yearOfRelease, description, userRating)
                                                                         VALUES (@gameId, @title, @slug, @yearOfRelease, @description, @userRating)
                                                                         """, game, cancellationToken: cancellationToken));

        if (result > 0)
        {
            foreach (var genre in game.Genres)
            {
                await connection.ExecuteAsync(new CommandDefinition("""
                                                                    INSERT INTO genres (gameId, name)
                                                                    VALUES (@gameId, @name)
                                                                    """, new { gameId = game.GameId, name = genre }, cancellationToken: cancellationToken));
            }
        }

        transaction.Commit();
        return result > 0;
    }

    public async Task<Game?> GetByIdAsync(Guid gameId, Guid? userId = default, CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.GetConnectionAsync(cancellationToken);

        var game = await connection.QueryFirstOrDefaultAsync<Game>(new CommandDefinition("""
                                                                                         SELECT g.*, ROUND(AVG(r.rating), 2) as rating, myr.rating as UserRating
                                                                                         FROM games g
                                                                                         LEFT JOIN ratings r on g.gameId = r.gameId
                                                                                         LEFT JOIN ratings myr on g.gameId = myr.gameId AND myr.userId = @userId
                                                                                         WHERE g.gameId = @gameId
                                                                                         GROUP BY g.gameId, myr.rating
                                                                                         """, new { gameId, userId }, cancellationToken: cancellationToken));

        if (game == null) return null;

        var genres = await connection.QueryAsync<string>(new CommandDefinition("""
                                                                               SELECT name FROM genres
                                                                               WHERE gameId = @gameId
                                                                               """, new { gameId }, cancellationToken: cancellationToken));

        foreach (var genre in genres)
        {
            game.Genres.Add(genre);
        }

        return game;
    }

    public async Task<Game?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.GetConnectionAsync(cancellationToken);

        var game = await connection.QueryFirstOrDefaultAsync<Game>(new CommandDefinition("""
                                                                                         SELECT g.*, ROUND(AVG(r.rating), 2) as rating, myr.rating as UserRating
                                                                                         FROM games g
                                                                                         LEFT JOIN ratings r on g.gameId = r.gameId
                                                                                         LEFT JOIN ratings myr on g.gameId = myr.gameId AND myr.userId = @userId
                                                                                         WHERE g.slug = @slug
                                                                                         GROUP BY g.gameId, myr.rating
                                                                                         """, new { slug, userId }, cancellationToken: cancellationToken));

        if (game == null) return null;

        var genres = await connection.QueryAsync<string>(new CommandDefinition("""
                                                                               SELECT name FROM genres
                                                                               WHERE gameId = @gameId
                                                                               """, new { gameId = game.GameId  }, cancellationToken: cancellationToken));

        foreach (var genre in genres)
        {
            game.Genres.Add(genre);
        }

        return game;
    }

    async Task<IEnumerable<Game>> IGamesRepository.GetAllAsync(GetAllGameOptions gameOptions, CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.GetConnectionAsync(cancellationToken);

        var orderClause = string.Empty;

        if (gameOptions.SortField != null)
        {
            orderClause = $"""
                           , g.{gameOptions.SortField}
                           ORDER BY g.{gameOptions.SortField} {(gameOptions.SortOrder == SortOrder.Ascending ? "ASC" : "DESC")}
                           """;
        }

        var games = await connection.QueryAsync(new CommandDefinition($"""
                                                                       SELECT g.*, 
                                                                              string_agg(DISTINCT gen.name, ', ') as genres,
                                                                              ROUND(AVG(r.rating), 1) as rating, 
                                                                              MAX(myr.rating) as myuserrating
                                                                       FROM games g
                                                                       LEFT JOIN genres gen ON g.gameId = gen.gameId
                                                                       LEFT JOIN ratings r ON g.gameId = r.gameId
                                                                       LEFT JOIN ratings myr ON g.gameId = myr.gameId AND myr.userId = @userId 
                                                                       WHERE (@title IS NULL OR g.title LIKE ('%' || @title || '%'))
                                                                       AND (@yearOfRelease IS NULL OR g.yearOfRelease = @yearOfRelease)
                                                                       GROUP BY g.gameid {orderClause}
                                                                       LIMIT @pageSize
                                                                       OFFSET @pageOffset
                                                                       """, new
        {
            userId = gameOptions.UserId,
            title = gameOptions.Title,
            yearOfRelease = gameOptions.YearOfRelease,
            pageSize = gameOptions.PageSize,
            pageOffset = (gameOptions.Page - 1) * gameOptions.PageSize
        }, cancellationToken: cancellationToken));
        
        return games.Select(g => new Game
        {
            GameId = g.gameid,
            Title = g.title,
            YearOfRelease = g.yearofrelease,
            Description = g.description,
            Rating = (float?)g.rating,
            UserRating = (int?)g.myuserrating,
            Genres = Enumerable.ToList(g.genres.Split(','))
        });
    }

    public async Task<bool> UpdateAsync(Game game, CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.GetConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
                                                            DELETE FROM genres
                                                            WHERE gameId = @gameId
                                                            """, new { gameId = game.GameId }, cancellationToken: cancellationToken));

        foreach (string gameGenre in game.Genres)
        {
            await connection.ExecuteAsync(new CommandDefinition("""
                                                                INSERT INTO genres (gameId, name)
                                                                VALUES (@gameId, @name)
                                                                """, new { gameId = game.GameId, name = gameGenre }, cancellationToken: cancellationToken));
        }

        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                            UPDATE games SET slug = @slug,
                                                                                             title = @title,
                                                                                             description = @description,
                                                                                             yearOfRelease = @yearOfRelease,
                                                                                             userRating = @userRating
                                                                            WHERE gameId = @gameId
                                                                         """, game, cancellationToken: cancellationToken));

        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> DeleteByIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.GetConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
                                                            DELETE FROM genres
                                                            WHERE gameId = @gameId
                                                            """, new { gameId }, cancellationToken: cancellationToken));

        await connection.ExecuteAsync(new CommandDefinition("""
                                                            DELETE FROM games
                                                            WHERE gameId = @gameId
                                                            """, new { gameId }, cancellationToken: cancellationToken));

        transaction.Commit();
        return true;
    }

    public async Task<bool> ExistsByIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.GetConnectionAsync(cancellationToken);

        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition("""
                                                                               SELECT COUNT(1) FROM games
                                                                               WHERE gameId = @gameId
                                                                               """, new { gameId }, cancellationToken: cancellationToken));
    }

    public async Task<int> GetCountAsync(string? title = default, int? yearOfRelease = default, CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.GetConnectionAsync(cancellationToken);

        return await connection.QuerySingleAsync<int>(new CommandDefinition("""
                                                                            SELECT COUNT(gameid) FROM games
                                                                            WHERE (@title IS NULL OR title LIKE ('%' || @title || '%'))
                                                                            AND (@yearOfRelease IS NULL OR yearOfRelease = @yearOfRelease)
                                                                            """, new { title, yearOfRelease }, cancellationToken: cancellationToken));
    }
}