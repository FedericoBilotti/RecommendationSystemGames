using App.Dtos.Authentication.Request;
using App.Dtos.Authentication.Response;
using App.Interfaces.Authentication;
using App.Mappers;
using FluentValidation;
using RM.Domain.Entities;

namespace App.UseCases.Authentication;

public class AuthenticateUserUseCase(IAuthService authService, ITokenService tokenService, IValidator<User> userValidator) : IAuthenticateUserUseCase
{ 
    public async Task<UserResponseDto?> RegisterAsync(UserRequestDto userRequestDto)
    {
        User user = userRequestDto.MapToNewUser();
        
        await userValidator.ValidateAndThrowAsync(user);
        
        User? result = await authService.RegisterAsync(user);
        
        return result?.MapToUserResponse();
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