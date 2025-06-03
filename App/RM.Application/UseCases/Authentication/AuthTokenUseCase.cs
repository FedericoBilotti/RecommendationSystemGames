using App.Dtos.Authentication;
using App.Interfaces.Authentication;
using RM.Domain.Entities;

namespace App.UseCases.Authentication;

public class AuthTokenUseCase(ITokenService tokenService)
{
    public async Task<TokenResponseDto> CreateTokenResponse(User user)
    {
        return await tokenService.CreateTokenResponse(user);
    }
    
    public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto requestRefreshTokenDto)
    {
        return await tokenService.RefreshTokenAsync(requestRefreshTokenDto);
    }
}