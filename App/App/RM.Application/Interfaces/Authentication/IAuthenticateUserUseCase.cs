using App.RM.Application.Dtos;
using App.RM.Application.Dtos.Authentication;
using App.RM.Domain.Entities;

namespace App.RM.Application.Interfaces.Authentication;

public interface IAuthenticateUserUseCase
{
    Task<User?> RegisterAsync(UserRequestDto request);
    Task<TokenResponseDto?> LoginAsync(UserRequestDto request);
    Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);
}