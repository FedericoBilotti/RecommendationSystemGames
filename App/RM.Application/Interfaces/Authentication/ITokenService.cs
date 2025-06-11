using App.Dtos.Authentication.Request;
using App.Dtos.Authentication.Response;
using RM.Domain.Entities;

namespace App.Interfaces.Authentication;

public interface ITokenService
{
    Task<(TokenResponseDto, DateTime)> CreateTokenResponse(User user, CancellationToken cancellationToken = default);
    Task<(TokenResponseDto, DateTime)> RefreshTokenAsync(Token requestRefreshTokenDto, CancellationToken cancellationToken = default);
    void WriteAuthTokenAsHttpOnlyCookie(string cookieName, string token, DateTime expiration);
}