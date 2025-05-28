using App.RM.Application.Dtos;
using App.RM.Application.Interfaces;
using App.RM.Domain.Entities;

namespace App.RM.Application.UseCases;

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