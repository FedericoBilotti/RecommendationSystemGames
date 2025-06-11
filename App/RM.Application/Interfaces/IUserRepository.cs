using RM.Domain.Entities;

namespace App.Interfaces;

public interface IUserRepository
{
    Task<bool> CreateUserAsync(User user, CancellationToken cancellationToken = default);
    Task<User?> GetUserById(Guid userId, CancellationToken cancellationToken = default);
    Task<User?> GetUserByUsername(string username, CancellationToken cancellationToken = default);
    Task<User?> GetUserByEmail(string email, CancellationToken cancellationToken);
    Task<bool> UpdateUserAsync(User user, CancellationToken cancellationToken = default);
}