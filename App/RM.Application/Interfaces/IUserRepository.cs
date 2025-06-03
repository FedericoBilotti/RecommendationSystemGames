using App.RM.Application.Dtos.Authentication;
using RM.Domain.Entities;

namespace App.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserById(Guid userId);
    Task<User?> GetUserByUsername(UserRequestDto requestUserRequestDto);
}