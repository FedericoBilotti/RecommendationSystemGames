using App.Domain.Entities;
using App.Infrastructure.Models.Dtos;

namespace App.Application.Interfaces;

public interface IAuthService
{
    Task<User?> RegisterAsync(UserDto requestUserDto);
    Task<TokenResponeDto?> LoginAsync(UserDto requestUserDto);
    Task<TokenResponeDto?> RefreshTokenAsync(RefreshTokenRequestDto requestRefreshTokenDto);
}