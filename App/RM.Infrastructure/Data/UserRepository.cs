using App.Interfaces;
using Dapper;
using RM.Domain.Entities;

namespace RM.Infrastructure.Data;

public class UserRepository(IDbConnectionFactory context) : IUserRepository
{
    public async Task<User?> GetUserById(Guid userId, CancellationToken cancellationToken = default)
    {
        using var connection = await context.GetConnectionAsync(cancellationToken);

        var result = await connection.QueryFirstOrDefaultAsync<User>(new CommandDefinition("""
                                                                                           SELECT *
                                                                                           FROM users 
                                                                                           WHERE userId = @userId
                                                                                           """, new { userId }, cancellationToken: cancellationToken));
        
        return result;
        // return await context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
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
}