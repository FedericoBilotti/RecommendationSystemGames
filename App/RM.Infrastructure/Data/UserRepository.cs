using App.Interfaces;
using Dapper;
using RM.Domain.Entities;

namespace RM.Infrastructure.Data;

public class UserRepository(IDbConnectionFactory context) : IUserRepository
{
    public async Task<bool> CreateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        using var connection = await context.GetConnectionAsync(cancellationToken);

        int result = await connection.ExecuteAsync(new CommandDefinition("""
                                                                         INSERT INTO users (userid, email, username, hashedPassword, role)
                                                                         VALUES (@userId, @username, @email, @hashedPassword, @role)
                                                                         """, user, cancellationToken: cancellationToken));
        
        return result > 0;
    }

    public async Task<User?> GetUserById(Guid userId, CancellationToken cancellationToken = default)
    {
        using var connection = await context.GetConnectionAsync(cancellationToken);

        var result = await connection.QueryFirstOrDefaultAsync<User>(new CommandDefinition("""
                                                                                           SELECT *
                                                                                           FROM users 
                                                                                           WHERE userId = @userId
                                                                                           """, new { userId }, cancellationToken: cancellationToken));
        
        return result;
    }

    public async Task<User?> GetUserByUsername(string username, CancellationToken cancellationToken = default)
    {
        using var connection = await context.GetConnectionAsync(cancellationToken);

        var result = await connection.QueryFirstOrDefaultAsync<User>(new CommandDefinition("""
                                                                                           SELECT *
                                                                                           FROM users 
                                                                                           WHERE userId = @userId
                                                                                           """, new { username }, cancellationToken: cancellationToken));
        
        return result;
    }

    public async Task<User?> GetUserByEmail(string email, CancellationToken cancellationToken)
    {
        using var connection = await context.GetConnectionAsync(cancellationToken);

        var result = await connection.QueryFirstOrDefaultAsync<User>(new CommandDefinition("""
                                                                                     SELECT *
                                                                                     FROM users
                                                                                     WHERE email = @email
                                                                                     """, new { email }, cancellationToken: cancellationToken));

        return result;
    }
}