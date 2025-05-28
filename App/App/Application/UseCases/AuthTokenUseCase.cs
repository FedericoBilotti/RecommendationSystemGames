using App.Application.Dtos;
using App.Application.Interfaces;
using App.Domain.Entities;

namespace App.Application.UseCases;

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