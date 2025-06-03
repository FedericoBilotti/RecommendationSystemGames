using App.RM.Application.Dtos;
using App.RM.Application.Dtos.Authentication;
using App.RM.Application.Interfaces;
using App.RM.Application.Interfaces.Authentication;
using App.RM.Domain.Entities;

namespace App.RM.Application.UseCases.Authentication;

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