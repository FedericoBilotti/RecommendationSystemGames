using App.Application.Dtos;
using App.Application.UseCases;
using App.Domain.Entities;

namespace App.Application.Interfaces;

public interface IAuthService
{
    Task<User?> RegisterAsync(UserDto requestUserDto);
    Task<TokenResponseDto?> LoginAsync(UserDto requestUserDto);
}