using App.RM.Application.Dtos;
using App.RM.Application.Interfaces;
using App.RM.Domain.Entities;

namespace App.RM.Application.UseCases;

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