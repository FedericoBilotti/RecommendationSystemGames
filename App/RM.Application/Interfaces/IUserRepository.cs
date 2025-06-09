using RM.Domain.Entities;

namespace App.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserById(Guid userId, CancellationToken cancellationToken = default);
    Task<User?> GetUserByUsername(string username, CancellationToken cancellationToken = default);
}