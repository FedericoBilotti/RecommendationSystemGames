using App.RM.Application.Dtos.Authentication;
using App.RM.Application.Interfaces;
using App.RM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using RM.Infrastructure.Database;

namespace RM.Infrastructure.Data;

public class DatabaseUserRepository(AppDbContext context) : IUserRepository
{
    public async Task<User?> GetUserById(Guid userId)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<User?> GetUserByUsername(UserRequestDto requestUserRequestDto)
    {
        string usernameLower = requestUserRequestDto.Username.ToLower();
        return await context.Users.FirstOrDefaultAsync(u => u.Username == usernameLower);
    }
}