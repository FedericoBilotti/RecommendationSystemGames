using App.Dtos.Authentication;
using App.Dtos.Authentication.Request;
using App.Dtos.Authentication.Response;
using RM.Domain.Entities;

namespace App.Interfaces.Authentication;

public interface IAuthenticateUserUseCase
{
    Task<UserResponseDto?> RegisterAsync(UserRegisterRequestDto userLoginRequestDto, CancellationToken cancellationToken = default);
    Task<TokenResponseDto?> LoginAsync(UserLoginRequestDto userLoginRequestDto, CancellationToken cancellationToken = default);
    Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequestDto, CancellationToken cancellationToken = default);
}