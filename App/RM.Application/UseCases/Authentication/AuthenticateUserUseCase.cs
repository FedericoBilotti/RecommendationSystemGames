using App.Dtos.Authentication.Request;
using App.Dtos.Authentication.Response;
using App.Interfaces;
using App.Interfaces.Authentication;
using App.Mappers;
using Microsoft.AspNetCore.Identity;
using RM.Domain.Entities;

namespace App.UseCases.Authentication;

public class AuthenticateUserUseCase(
        IUserRepository userRepository,
        ITokenService tokenService,
        IPasswordHasher<UserRegisterRequestDto> hasher,
        IValidationService validationService) : IAuthenticateUserUseCase
{
    public async Task<UserResponseDto?> RegisterAsync(UserRegisterRequestDto userLoginRequestDto, CancellationToken cancellationToken = default)
    {
        User user = userLoginRequestDto.MapToUser(hasher);
        
        Console.WriteLine("antes");
        await validationService.ValidateUserAndThrowAsync(user, cancellationToken);
        Console.WriteLine("despues");

        bool result = await userRepository.CreateUserAsync(user, cancellationToken);

        return result ? user.MapToUserResponse() : null;
    }

    public async Task<TokenResponseDto?> LoginAsync(UserLoginRequestDto userLoginRequestDto, CancellationToken cancellationToken = default)
    {
        await validationService.ValidateLoginAndThrowAsync(userLoginRequestDto, cancellationToken);

        User? res = userLoginRequestDto.Email != null
                ? await userRepository.GetUserByEmail(userLoginRequestDto.Email, cancellationToken)
                : await userRepository.GetUserByUsername(userLoginRequestDto.Username!, cancellationToken);

        TokenResponseDto tokenResponseDto = await tokenService.CreateTokenResponse(res!, cancellationToken);

        return tokenResponseDto;
    }

    public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequestDto, CancellationToken cancellationToken = default)
    {
        await validationService.ValidateTokenAndThrowAsync(refreshTokenRequestDto, cancellationToken);
        
        return await tokenService.RefreshTokenAsync(refreshTokenRequestDto, cancellationToken);
    }
}