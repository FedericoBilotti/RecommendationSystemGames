using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using App.Application.Interfaces;
using App.Domain.Entities;
using App.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace App.Application.UseCases;

public class AuthenticateUserUseCase(
        IConfiguration configuration, 
        AppDbContext context, 
        IUserRepository userRepository, 
        IPasswordHasher<User> hasher, 
        ITokenService tokens) 
        : IAuthService
{
    public async Task<User?> RegisterAsync(UserDto requestUserDto)
    {
        if (await userRepository.GetUserByUsername(context, requestUserDto) != null) return null;

        var user = new User();

        string hashedPassword = new PasswordHasher<User>().HashPassword(user, requestUserDto.Password);
        user.Username = requestUserDto.Username;
        user.HashedPassword = hashedPassword;

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user;
    }

    public async Task<TokenResponseDto?> LoginAsync(UserDto requestUserDto)
    {
        var user = await userRepository.GetUserByUsername(context, requestUserDto);

        if (user == null) return null;

        PasswordVerificationResult passwordVerificationResult = new PasswordHasher<User>().VerifyHashedPassword(user, user.HashedPassword, requestUserDto.Password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed) return null;

        return await CreateTokenResponse(user);
    }

    public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto requestRefreshTokenDto)
    {
        User? user = await ValidateRefreshToken(requestRefreshTokenDto.UserId, requestRefreshTokenDto.RefreshToken);

        if (user == null) return null;

        return await CreateTokenResponse(user);
    }

    private async Task<TokenResponseDto> CreateTokenResponse(User user)
    {
        var response = new TokenResponseDto
        {
            AccessToken = GenerateToken(user),
            RefreshToken = await GenerateAndSaveRefreshToken(user)
        };
        return response;
    }

    private async Task<User?> ValidateRefreshToken(Guid userId, string refreshToken)
    {
        User? user = await userRepository.GetUserById(context, userId);
        bool isNotValid = user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpirationTime < DateTime.UtcNow;

        return isNotValid ? null : user;
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private async Task<string> GenerateAndSaveRefreshToken(User user)
    {
        var refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpirationTime = DateTime.UtcNow.AddMinutes(30);
        await context.SaveChangesAsync();
        return refreshToken;
    }

    private string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
        var tokenDescriptor = new JwtSecurityToken(issuer: configuration.GetValue<string>("AppSettings:Issuer"), audience: configuration.GetValue<string>("AppSettings:Audience"), claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30), signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
}