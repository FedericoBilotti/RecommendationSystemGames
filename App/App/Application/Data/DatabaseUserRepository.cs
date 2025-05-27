using App.Application.Dtos;
using App.Application.Interfaces;
using App.Application.UseCases;
using App.Domain.Entities;
using App.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace App.Application.Data;

public class DatabaseUserRepository : IUserRepository
{
    public async Task<User?> GetUserById(AppDbContext context, Guid userId)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<User?> GetUserByUsername(AppDbContext context, UserDto requestUserDto)
    {
        string usernameLower = requestUserDto.Username.ToLower();
        return await context.Users.FirstOrDefaultAsync(u => u.Username == usernameLower);
    }
}