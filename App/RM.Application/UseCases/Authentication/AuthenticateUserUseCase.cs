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
        IUserValidationService userValidationService) : IAuthenticateUserUseCase
{
    public async Task<UserResponseDto?> RegisterAsync(UserRegisterRequestDto userRegisterRequestDto, CancellationToken cancellationToken = default)
    {
        await userValidationService.ValidateUserAndThrowAsync(userRegisterRequestDto, cancellationToken);

        string password = hasher.HashPassword(userRegisterRequestDto, userRegisterRequestDto.Password);
        User user = userRegisterRequestDto.MapToUser(password);

        bool result = await userRepository.CreateUserAsync(user, cancellationToken);

        return result ? user.MapToUserResponse() : null;
    }

    public async Task<TokenResponseDto?> LoginAsync(UserLoginRequestDto userLoginRequestDto, CancellationToken cancellationToken = default)
    {
        await userValidationService.ValidateLoginAndThrowAsync(userLoginRequestDto, cancellationToken);

        User? user = await GetUser(userLoginRequestDto, cancellationToken);

        if (user == null)
            return null;

        // Must be provided in the constructor
        //
        var s = new PasswordHasher<User>();
        var passwordResult = s.VerifyHashedPassword(user, user.HashedPassword, userLoginRequestDto.Password);

        if (passwordResult == PasswordVerificationResult.Failed)
        {
            return null;
        }
        //

        Console.WriteLine($"User after login: {user.Username}");
        TokenResponseDto tokenResponseDto = await tokenService.CreateTokenResponse(user, cancellationToken);
        Console.WriteLine($"Token response: {tokenResponseDto.AccessToken}");
        Console.WriteLine($"Token response: {tokenResponseDto.RefreshToken}");

        return tokenResponseDto;
    }

    public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequestDto, CancellationToken cancellationToken = default)
    {
        await userValidationService.ValidateTokenAndThrowAsync(refreshTokenRequestDto, cancellationToken);
        
        return await tokenService.RefreshTokenAsync(refreshTokenRequestDto, cancellationToken);
    }

    private async Task<User?> GetUser(UserLoginRequestDto userLoginRequestDto, CancellationToken cancellationToken)
    {
        return userLoginRequestDto.Email != null
                ? await userRepository.GetUserByEmail(userLoginRequestDto.Email, cancellationToken)
                : await userRepository.GetUserByUsername(userLoginRequestDto.Username!, cancellationToken);
    }
}