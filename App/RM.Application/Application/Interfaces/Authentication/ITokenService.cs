using App.RM.Application.Dtos;
using App.RM.Application.Dtos.Authentication;
using App.RM.Domain.Entities;

namespace App.RM.Application.Interfaces.Authentication;

public interface ITokenService
{
    Task<TokenResponseDto> CreateTokenResponse(User user);
    Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto requestRefreshTokenDto);
}