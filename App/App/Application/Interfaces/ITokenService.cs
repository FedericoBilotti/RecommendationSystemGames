using App.Application.Dtos;
using App.Application.UseCases;
using App.Domain.Entities;

namespace App.Application.Interfaces;

public interface ITokenService
{
    Task<TokenResponseDto> CreateTokenResponse(User user);
    Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto requestRefreshTokenDto);
}