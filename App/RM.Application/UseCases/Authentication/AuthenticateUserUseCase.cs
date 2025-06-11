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
        IHasherService hasherServiceService,
        IUserValidationService userValidationService) : IAuthenticateUserUseCase
{
    public async Task<UserResponseDto?> RegisterAsync(UserRegisterRequestDto userRegisterRequestDto, CancellationToken cancellationToken = default)
    {
        await userValidationService.ValidateUserAndThrowAsync(userRegisterRequestDto, cancellationToken);

        string password = hasherServiceService.RegisterHasher(userRegisterRequestDto);
        User user = userRegisterRequestDto.MapToUser(password);

        bool result = await userRepository.CreateUserAsync(user, cancellationToken);

        return result ? user.MapToUserResponse() : null;
    }

    // Maybe not pass the refresh token as dto, cause' it's valuable 
    public async Task<TokenResponseDto?> LoginAsync(UserLoginRequestDto userLoginRequestDto, CancellationToken cancellationToken = default)
    {
        await userValidationService.ValidateLoginAndThrowAsync(userLoginRequestDto, cancellationToken);

        User? user = await GetUser(userLoginRequestDto, cancellationToken);

        if (user == null)
            return null;

        var passwordResult = hasherServiceService.LoginHasher(user, userLoginRequestDto);
        if (passwordResult == PasswordVerificationResult.Failed)
        {
            return null;
        }

        TokenResponseDto tokenResponseDto = await CreateToken(cancellationToken, user);

        return tokenResponseDto;
    }

    private async Task<TokenResponseDto> CreateToken(CancellationToken cancellationToken, User user)
    {
        (TokenResponseDto tokenResponseDto, DateTime expirationDateTimeUtc) = await tokenService.CreateTokenResponse(user, cancellationToken);
        DateTime refreshTokenExpirationDateTimeUtc = DateTime.UtcNow.AddDays(7); // Must be provided in other place ¿?
        
        user.RefreshToken = tokenResponseDto.RefreshToken;
        user.RefreshTokenExpirationTimeUtc = refreshTokenExpirationDateTimeUtc;
        
        await UpdateUserAsync(user, cancellationToken);
        
        tokenService.WriteAuthTokenAsHttpOnlyCookie(TokenConstants.ACCESS_TOKEN, tokenResponseDto.AccessToken, expirationDateTimeUtc);
        tokenService.WriteAuthTokenAsHttpOnlyCookie(TokenConstants.REFRESH_TOKEN, tokenResponseDto.RefreshToken, refreshTokenExpirationDateTimeUtc);
        return tokenResponseDto;
    }

    public async Task<(TokenResponseDto, DateTime)> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequestDto, CancellationToken cancellationToken = default)
    {
        await userValidationService.ValidateTokenAndThrowAsync(refreshTokenRequestDto, cancellationToken);
        
        var token = refreshTokenRequestDto.MapToToken();
        
        return await tokenService.RefreshTokenAsync(token, cancellationToken);
    }

    private async Task<User?> GetUser(UserLoginRequestDto userLoginRequestDto, CancellationToken cancellationToken = default)
    {
        return userLoginRequestDto.Email != null
                ? await userRepository.GetUserByEmail(userLoginRequestDto.Email, cancellationToken)
                : await userRepository.GetUserByUsername(userLoginRequestDto.Username!, cancellationToken);
    }

    private async Task UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        await userRepository.UpdateUserAsync(user, cancellationToken);
    }
}