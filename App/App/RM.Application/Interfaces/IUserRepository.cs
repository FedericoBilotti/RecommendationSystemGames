using App.RM.Application.Dtos;
using App.RM.Domain.Entities;
using App.RM.Infrastructure.Database;

namespace App.RM.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserById(AppDbContext context, Guid userId);
    Task<User?> GetUserByUsername(AppDbContext context, UserRequestDto requestUserRequestDto);
}