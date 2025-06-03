using App.Interfaces;
using Microsoft.EntityFrameworkCore;
using RM.Domain.Entities;
using RM.Infrastructure.Database;

namespace RM.Infrastructure.Data;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<User?> GetUserById(Guid userId)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<User?> GetUserByUsername(User requestUserRequestDto)
    {
        string usernameLower = requestUserRequestDto.Username.ToLower();
        return await context.Users.FirstOrDefaultAsync(u => u.Username == usernameLower);
    }
}