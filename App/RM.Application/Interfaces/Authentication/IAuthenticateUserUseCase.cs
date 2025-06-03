using App.RM.Application.Dtos.Authentication;
using RM.Domain.Entities;

namespace App.Interfaces.Authentication;

public interface IAuthenticateUserUseCase
{
    Task<User?> RegisterAsync(UserRequestDto request);
    Task<TokenResponseDto?> LoginAsync(UserRequestDto request);
    Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);
}