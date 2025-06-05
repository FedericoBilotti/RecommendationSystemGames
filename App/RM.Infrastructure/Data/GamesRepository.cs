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
            foreach (var genre in game.Genres)
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

    public async Task<Game?> GetByIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.GetConnectionAsync();

        var game = await connection.QueryFirstOrDefaultAsync<Game>(new CommandDefinition("""
                                                                                         SELECT * FROM games
                                                                                         WHERE gameId = @gameId
                                                                                         """, new { gameId }));
        
        if (game == null) return null;

        var genres = await connection.QueryAsync<string>(new CommandDefinition("""
                                                                               SELECT name FROM genres
                                                                               WHERE gameId = @gameId
                                                                               """, new { gameId }));
        
        foreach (var genre in genres)
        {
            game.Genres.Add(genre);
        }

        return game;
    }

    public async Task<Game?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.GetConnectionAsync();

        var game = await connection.QueryFirstOrDefaultAsync<Game>(new CommandDefinition("""
                                                                                         SELECT * FROM games
                                                                                         WHERE slug = @slug
                                                                                         """, new { slug }));
        
        if (game == null) return null;

        var genres = await connection.QueryAsync<string>(new CommandDefinition("""
                                                                              SELECT name FROM genres
                                                                              WHERE gameId = @gameId
                                                                              """, new { game.Slug }));
        
        foreach (var genre in genres)
        {
            game.Genres.Add(genre);
        }

        return game;
    }

    public async Task<IEnumerable<Game>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.GetConnectionAsync();

        var games = await connection.QueryAsync(new CommandDefinition("""
                                                                            SELECT gam.*, string_agg(genres.name, ', ') as genres
                                                                            FROM games as gam
                                                                            LEFT JOIN genres as genres 
                                                                                ON gam.gameId = genres.gameId
                                                                            GROUP BY gam.gameId
                                                                            """));
        
        return games.Select(g => new Game
        {
            GameId = g.gameid,
            Title = g.title,
            YearOfRelease = g.yearofrelease,
            Description = g.description,
            Genres = Enumerable.ToList(g.genres.Split(','))
        });
    }

    public async Task<bool> UpdateAsync(Game game, CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.GetConnectionAsync();
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
                                                            DELETE FROM genres
                                                            WHERE gameId = @gameId
                                                            """, new { gameId = game.GameId }));

        foreach (string gameGenre in game.Genres)
        {
            await connection.ExecuteAsync(new CommandDefinition("""
                                                                INSERT INTO genres (gameId, name)
                                                                VALUES (@gameId, @name)
                                                                """, new { gameId = game.GameId, name = gameGenre }));
        }

        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                            UPDATE games SET slug = @slug,
                                                                                             title = @title,
                                                                                             description = @description,
                                                                                             yearOfRelease = @yearOfRelease,
                                                                                             userRating = @userRating
                                                                            WHERE gameId = @gameId
                                                                         """, game));
        
        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> DeleteByIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.GetConnectionAsync();
        using var transaction = connection.BeginTransaction();
        
        await connection.ExecuteAsync(new CommandDefinition("""
                                                            DELETE FROM genres
                                                            WHERE gameId = @gameId
                                                            """, new { gameId }));
        
        await connection.ExecuteAsync(new CommandDefinition("""
                                                            DELETE FROM games
                                                            WHERE gameId = @gameId
                                                            """, new { gameId }));

        transaction.Commit();
        return true;
    }

    public async Task<bool> ExistsByIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.GetConnectionAsync();

        return await connection.ExecuteScalarAsync<bool>(new CommandDefinition("""
                                                                               SELECT COUNT(1) FROM games
                                                                               WHERE gameId = @gameId
                                                                               """, new { gameId }));
    }
}