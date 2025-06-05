using App.Interfaces;
using Dapper;

namespace RM.Infrastructure.Database;

public class DbInitializer(IDbConnectionFactory dbConnectionFactory)
{
    public async Task InitializeDbAsync()
    {
        using var connection = await dbConnectionFactory.GetConnectionAsync();
        await connection.ExecuteAsync("""
                                      CREATE TABLE IF NOT EXISTS games (
                                      gameId UUID PRIMARY KEY,
                                      title TEXT NOT NULL,
                                      slug TEXT NOT NULL,
                                      yearOfRelease INTEGER NOT NULL,
                                      userRating INTEGER);
                                      description TEXT NOT NULL,
                                      """);
        
        await connection.ExecuteAsync("""
                                      CREATE UNIQUE INDEX CONCURRENTLY IF NOT EXISTS games_slug_uindex
                                      ON games
                                      USING btree(slug);
                                      """);
        
        
    }
}