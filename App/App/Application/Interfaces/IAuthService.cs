using App.Application.Dtos;
using App.Domain.Entities;

namespace App.Application.Interfaces;

public interface IAuthService
{
    Task<User?> RegisterAsync(UserRequestDto requestUserRequestDto);
    Task<TokenResponseDto?> LoginAsync(UserRequestDto requestUserRequestDto);
}