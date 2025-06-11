using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using App.Dtos.Authentication.Request;
using App.Dtos.Authentication.Response;
using App.Interfaces;
using App.Interfaces.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RM.Domain.Entities;

namespace App.Services.Authenticate;

public class TokenService(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : ITokenService
{
    public async Task<TokenResponseDto> CreateTokenResponse(User user, CancellationToken cancellationToken = default)
    {
        (string? handler, DateTime expiresTime) generatedToken = GenerateToken(user);
        
        if (generatedToken.handler == null)
            throw new Exception("Error generating token");
        
        return new TokenResponseDto
        {
            AccessToken = generatedToken.handler,
            RefreshToken = await GenerateAndSaveRefreshToken(user, generatedToken.handler, generatedToken.expiresTime, cancellationToken)
        };
    }

    public async Task<TokenResponseDto?> RefreshTokenAsync(Token requestRefreshTokenDto, CancellationToken cancellationToken = default)
    {
        User? user = await ValidateRefreshToken(requestRefreshTokenDto.UserId, requestRefreshTokenDto.RefreshToken);
        
        if (user == null) 
            return null;
        
        return await CreateTokenResponse(user, cancellationToken);
    }

    public void WriteAuthTokenAsHttpOnlyCookie(string cookieName, string token, DateTime expiration)
    {
        httpContextAccessor.HttpContext?.Response.Cookies.Append(cookieName, token, new CookieOptions
        {
            HttpOnly = true, 
            Expires = expiration, 
            IsEssential = true, 
            Secure = true, 
            SameSite = SameSiteMode.Strict
        });
    }

    private async Task<User?> ValidateRefreshToken(Guid userId, string refreshToken)
    {
        User? user = await userRepository.GetUserById(userId);
        bool isNotValid = user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpirationTimeUtc < DateTime.UtcNow;
        return isNotValid ? null : user;
    }

    private async Task<string> GenerateAndSaveRefreshToken(User user, string accessToken, DateTime expirationDateTimeUtc, CancellationToken cancellationToken = default)
    {
        string refreshToken = GenerateRefreshToken();
        DateTime refreshTokenExpirationDateTimeUtc = DateTime.UtcNow.AddDays(7); // Must be provided in other place ¿?
        
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpirationTimeUtc = refreshTokenExpirationDateTimeUtc;
        
        await UpdateUserAsync(user, cancellationToken);
        
        WriteAuthTokenAsHttpOnlyCookie(TokenConstants.ACCESS_TOKEN, accessToken, expirationDateTimeUtc);
        WriteAuthTokenAsHttpOnlyCookie(TokenConstants.REFRESH_TOKEN, user.RefreshToken, refreshTokenExpirationDateTimeUtc);

        return refreshToken;
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private (string?, DateTime) GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Role, user.Role)
        };

        DateTime expires = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("AppSettings:ExpiresInMinutes"));

        // Change symmetric key?
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
        
        var token = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"), 
                audience: configuration.GetValue<string>("AppSettings:Audience"), 
                claims: claims,
                expires: expires,
                signingCredentials: credentials);

        string? handler = new JwtSecurityTokenHandler().WriteToken(token);

        return (handler, expires);
    }

    private async Task UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        await userRepository.UpdateUserAsync(user, cancellationToken);
    }
}