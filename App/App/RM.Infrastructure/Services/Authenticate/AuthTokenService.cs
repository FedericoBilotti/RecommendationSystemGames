using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using App.RM.Application.Dtos.Authentication;
using App.RM.Application.Interfaces;
using App.RM.Application.Interfaces.Authentication;
using App.RM.Domain.Entities;
using App.RM.Infrastructure.Database;
using Microsoft.IdentityModel.Tokens;

namespace App.RM.Infrastructure.Services.Authenticate;

public class AuthTokenService(AppDbContext context, IUserRepository userRepository, IConfiguration configuration) : ITokenService
{
    public async Task<TokenResponseDto> CreateTokenResponse(User user)
    {
        return new TokenResponseDto
        {
            AccessToken = GenerateToken(user),
            RefreshToken = await GenerateAndSaveRefreshToken(user)
        };
    }
    
    public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto requestRefreshTokenDto)
    {
        User? user = await ValidateRefreshToken(requestRefreshTokenDto.UserId, requestRefreshTokenDto.RefreshToken);

        if (user == null) return null;

        return await CreateTokenResponse(user);
    }

    private async Task<User?> ValidateRefreshToken(Guid userId, string refreshToken)
    {
        User? user = await userRepository.GetUserById(context, userId);
        bool isNotValid = user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpirationTime < DateTime.UtcNow;

        return isNotValid ? null : user;
    }

    private async Task<string> GenerateAndSaveRefreshToken(User user)
    {
        string refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpirationTime = DateTime.UtcNow.AddMinutes(30);
        await context.SaveChangesAsync();
        return refreshToken;
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
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