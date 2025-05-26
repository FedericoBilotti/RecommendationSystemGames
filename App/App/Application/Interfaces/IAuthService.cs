using App.Domain.Entities;
using App.Infrastructure.Models.User;

namespace App.Application.Interfaces;

public interface IAuthService
{
    Task<User?> RegisterAsync(UserDto requestUserDto);
    Task<string?> LoginAsync(UserDto requestUserDto);
}