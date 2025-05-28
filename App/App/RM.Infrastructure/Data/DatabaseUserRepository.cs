using App.RM.Application.Dtos;
using App.RM.Application.Interfaces;
using App.RM.Domain.Entities;
using App.RM.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace App.RM.Infrastructure.Data;

public class DatabaseUserRepository : IUserRepository
{
    public async Task<User?> GetUserById(AppDbContext context, Guid userId)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<User?> GetUserByUsername(AppDbContext context, UserRequestDto requestUserRequestDto)
    {
        string usernameLower = requestUserRequestDto.Username.ToLower();
        return await context.Users.FirstOrDefaultAsync(u => u.Username == usernameLower);
    }
}