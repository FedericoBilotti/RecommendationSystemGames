using App.Application.Dtos;
using App.Application.UseCases;
using App.Domain.Entities;
using App.Infrastructure.Database;

namespace App.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserById(AppDbContext context, Guid userId);
    Task<User?> GetUserByUsername(AppDbContext context, UserRequestDto requestUserRequestDto);
}