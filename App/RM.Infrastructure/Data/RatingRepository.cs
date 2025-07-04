using App.Interfaces;
using App.Interfaces.Engine;
using Dapper;
using RM.Domain.Entities.Games;

namespace RM.Infrastructure.Data;

public class RatingRepository(IDbConnectionFactory connectionFactory) : IRatingRepository
{
    public async Task<bool> RateGameAsync(Guid gameId, int rating, Guid userId, CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.GetConnectionAsync(cancellationToken);

        var result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                            INSERT INTO ratings (gameId, userId, rating)
                                                                            VALUES (@gameId, @userId, @rating)
                                                                            ON CONFLICT (gameId, userId) 
                                                                            DO UPDATE SET rating = @rating
                                                                         """, new { gameId, userId, rating }, cancellationToken: cancellationToken));

        return result > 0;
    }

    public async Task<float?> GetRatingAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.GetConnectionAsync(cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<float?>(new CommandDefinition("""
                                                                                        SELECT ROUND(AVG(r.rating), 1)
                                                                                        FROM ratings r
                                                                                        WHERE gameId = @gameId
                                                                                        """, new { gameId }, cancellationToken: cancellationToken));
    }

    public async Task<(float? Rating, int? User)> GetUserRatingAsync(Guid gameId, Guid userId, CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.GetConnectionAsync(cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<(float? Rating, int? User)>(new CommandDefinition("""
                                                                                                           SELECT ROUND(AVG(rating), 1),
                                                                                                               (SELECT rating 
                                                                                                                FROM ratings 
                                                                                                                WHERE gameId = @gameId AND userId = @userId
                                                                                                                LIMIT 1)
                                                                                                           FROM ratings 
                                                                                                           WHERE gameId = @gameId
                                                                                                           """, new { gameId, userId }, cancellationToken: cancellationToken));
    }

    public async Task<bool> DeleteRatingAsync(Guid gameId, Guid userId, CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.GetConnectionAsync(cancellationToken);

        int result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         DELETE FROM ratings
                                                                         WHERE gameId = @gameId AND userId = @userId
                                                                         """, new { gameId, userId }, cancellationToken: cancellationToken));

        return result > 0;
    }

    public async Task<IEnumerable<GameRating>> GetUserRatingsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        using var connection = await connectionFactory.GetConnectionAsync(cancellationToken);

        return await connection.QueryAsync<GameRating>(new CommandDefinition("""
                                                                        SELECT rat.rating, rat.gameId, g.slug
                                                                        FROM ratings rat
                                                                        INNER JOIN games g ON rat.gameId = g.gameId
                                                                        WHERE rat.userId = @userId
                                                                        """, new { userId }, cancellationToken: cancellationToken));
    }
}