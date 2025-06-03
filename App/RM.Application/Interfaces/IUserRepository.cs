using App.RM.Application.Dtos.Authentication;
using RM.Domain.Entities;

namespace App.RM.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserById(Guid userId);
    Task<User?> GetUserByUsername(UserRequestDto requestUserRequestDto);
}