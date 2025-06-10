using App.Dtos.Authentication;
using App.Dtos.Authentication.Request;
using App.Dtos.Authentication.Response;
using App.Interfaces.Authentication;
using RM.Domain.Entities;

namespace App.UseCases.Authentication;

public class AuthTokenUseCase(ITokenService tokenService)
{
    public async Task<TokenResponseDto> CreateTokenResponse(User user, CancellationToken cancellationToken = default)
    {
        return await tokenService.CreateTokenResponse(user, cancellationToken);
    }
    
    public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto requestRefreshTokenDto, CancellationToken cancellationToken = default)
    {
        return await tokenService.RefreshTokenAsync(requestRefreshTokenDto, cancellationToken);
    }
}