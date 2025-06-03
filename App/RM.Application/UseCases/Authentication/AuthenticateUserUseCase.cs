using App.Dtos.Authentication;
using App.Interfaces.Authentication;
using RM.Domain.Entities;

namespace App.UseCases.Authentication;

public class AuthenticateUserUseCase(IAuthService authService, ITokenService tokenService) : IAuthenticateUserUseCase
{
    async Task<User?> IAuthenticateUserUseCase.RegisterAsync(UserRequestDto request)
    {
        return await authService.RegisterAsync(request);
    }

    public async Task<TokenResponseDto?> LoginAsync(UserRequestDto requestUserRequestDto)
    {
        return await authService.LoginAsync(requestUserRequestDto);
    }

    public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        return await tokenService.RefreshTokenAsync(request);
    }
}