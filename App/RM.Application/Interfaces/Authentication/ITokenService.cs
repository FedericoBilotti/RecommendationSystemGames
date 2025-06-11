using App.Dtos.Authentication.Request;
using App.Dtos.Authentication.Response;
using RM.Domain.Entities;

namespace App.Interfaces.Authentication;

public interface ITokenService
{
    Task<TokenResponseDto> CreateTokenResponse(User user, CancellationToken cancellationToken = default);
    Task<TokenResponseDto?> RefreshTokenAsync(Token requestRefreshTokenDto, CancellationToken cancellationToken = default);
}