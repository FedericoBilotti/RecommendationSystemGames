using App.Dtos.Authentication;
using App.Dtos.Authentication.Request;
using App.Dtos.Authentication.Response;
using RM.Domain.Entities;

namespace App.Interfaces.Authentication;

public interface IAuthenticateUserUseCase
{
    Task<User?> RegisterAsync(UserRequestDto request);
    Task<TokenResponseDto?> LoginAsync(UserRequestDto request);
    Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);
}