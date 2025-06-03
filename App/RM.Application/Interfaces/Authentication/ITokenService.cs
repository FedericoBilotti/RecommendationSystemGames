using App.Dtos.Authentication;
using RM.Domain.Entities;

namespace App.Interfaces.Authentication;

public interface ITokenService
{
    Task<TokenResponseDto> CreateTokenResponse(User user);
    Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto requestRefreshTokenDto);
}