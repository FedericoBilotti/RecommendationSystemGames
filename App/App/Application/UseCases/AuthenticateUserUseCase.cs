using App.Application.Dtos;
using App.Application.Interfaces;
using App.Domain.Entities;

namespace App.Application.UseCases;

public class AuthenticateUserUseCase(IAuthService authService)
{
    public async Task<User?> RegisterAsync(UserDto requestUserDto)
    {
        return await authService.RegisterAsync(requestUserDto);
    }

    public async Task<TokenResponseDto?> LoginAsync(UserDto requestUserDto)
    {
        return await authService.LoginAsync(requestUserDto);
    }
}