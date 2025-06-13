using App.Interfaces;
using Dapper;

namespace RM.Infrastructure.Database;

public class DbInitializer(IDbConnectionFactory dbConnectionFactory)
{
    public async Task InitializeDbAsync()
    {
        await Task.WhenAll([CreateUserTables(), CreateGameTables()]);
    }

    private async Task CreateUserTables()
    {
        using var connection = await dbConnectionFactory.GetConnectionAsync();

        await connection.ExecuteAsync("""
                                      CREATE TABLE IF NOT EXISTS users (
                                          userId UUID PRIMARY KEY,
                                          email TEXT NOT NULL,
                                          username TEXT NOT NULL,
                                          hashedPassword TEXT NOT NULL,
                                          role TEXT NOT NULL,
                                          trustedUser BOOLEAN NOT NULL,
                                          refreshToken TEXT,
                                          refreshTokenExpirationTimeUtc TIMESTAMP WITH TIME ZONE
                                      )
                                      """);
        
        await connection.ExecuteAsync("""
                                      CREATE UNIQUE INDEX IF NOT EXISTS users_username_uindex
                                      ON users
                                      USING btree(username);
                                      """);
        
        await connection.ExecuteAsync("""
                                      CREATE UNIQUE INDEX IF NOT EXISTS users_email_uindex
                                      ON users
                                      USING btree(email);
                                      """);
    }

    private async Task CreateGameTables()
    {
        using var connection = await dbConnectionFactory.GetConnectionAsync();
        await connection.ExecuteAsync("""
                                      CREATE TABLE IF NOT EXISTS games (
                                      gameId UUID PRIMARY KEY,
                                      title TEXT NOT NULL,
                                      slug TEXT NOT NULL,
                                      yearOfRelease INTEGER NOT NULL,
                                      description TEXT NOT NULL,
                                      userRating INTEGER);
                                      """);

        await connection.ExecuteAsync("""
                                      CREATE UNIQUE INDEX CONCURRENTLY IF NOT EXISTS games_slug_uindex
                                      ON games
                                      USING btree(slug);
                                      """);

        await connection.ExecuteAsync("""
                                      CREATE TABLE IF NOT EXISTS genres (
                                      gameId UUID REFERENCES games(gameId),
                                      name TEXT NOT NULL);
                                      """);

        await connection.ExecuteAsync("""
                                      CREATE TABLE IF NOT EXISTS ratings (
                                      userId UUID,
                                      gameId UUID REFERENCES games(gameId),
                                      rating INTEGER NOT NULL,
                                      PRIMARY KEY (userId, gameId));
                                      """);
    }
}