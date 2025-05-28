using App.Application.Dtos;
using App.Application.Interfaces;
using App.Domain.Entities;

namespace App.Application.UseCases;

public class AuthenticateUserUseCase(IAuthService authService)
{
    public async Task<User?> RegisterAsync(UserRequestDto requestUserRequestDto)
    {
        return await authService.RegisterAsync(requestUserRequestDto);
    }

    public async Task<TokenResponseDto?> LoginAsync(UserRequestDto requestUserRequestDto)
    {
        return await authService.LoginAsync(requestUserRequestDto);
    }
}