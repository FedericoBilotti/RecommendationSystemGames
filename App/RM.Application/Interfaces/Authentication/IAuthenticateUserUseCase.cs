using App.Dtos.Authentication.Request;
using App.Dtos.Authentication.Response;

namespace App.Interfaces.Authentication;

public interface IAuthenticateUserUseCase
{
    Task<UserResponseDto?> RegisterAsync(UserRegisterRequestDto userRegisterRequestDto, CancellationToken cancellationToken = default);
    Task<TokenResponseDto?> LoginAsync(UserLoginRequestDto userLoginRequestDto, CancellationToken cancellationToken = default);
    Task<(TokenResponseDto tokenResponseDto, DateTime expirationDateTimeUtc)> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequestDto, CancellationToken cancellationToken = default);
}