using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using App.Auth;
using App.Dtos.Authentication.Request;
using App.Dtos.Authentication.Response;
using App.Interfaces;
using App.Interfaces.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RM.Domain.Entities;

namespace App.Services.Authenticate;

public class TokenService : ITokenService
{
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenService(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
    }
    
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

    public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto requestRefreshTokenDto, CancellationToken cancellationToken = default)
    {
        User? user = await ValidateRefreshToken(requestRefreshTokenDto.UserId, requestRefreshTokenDto.RefreshToken);
        
        if (user == null) 
            return null;
        
        return await CreateTokenResponse(user, cancellationToken);
    }

    private async Task<User?> ValidateRefreshToken(Guid? userId, string? refreshToken)
    {
        if (userId == null) return null;
        
        User? user = await _userRepository.GetUserById((Guid)userId);
        bool isValid = user != null && user.RefreshToken == refreshToken && user.RefreshTokenExpirationTimeUtc >= DateTime.UtcNow;
        return isValid ? user : null;
    }

    private async Task<string> GenerateAndSaveRefreshToken(User user, string accessToken, DateTime expirationDateTimeUtc, CancellationToken cancellationToken = default)
    {
        string refreshToken = GenerateRefreshToken();
        DateTime refreshTokenExpirationDateTimeUtc = DateTime.UtcNow.AddDays(7);
        
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpirationTimeUtc = refreshTokenExpirationDateTimeUtc;
        
        await UpdateUserAsync(user, cancellationToken);

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
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Role, user.Role),
            new("userid", user.UserId.ToString()),
            new(AuthConstants.ADMIN_CLAIM, user.Role == AuthConstants.ADMIN_ROLE ? "true" : "false", ClaimValueTypes.String),
            new(AuthConstants.TRUSTED_CLAIM, user.Role switch
            {
                AuthConstants.ADMIN_ROLE   => "true",
                AuthConstants.TRUSTED_ROLE => "true",
                _                          => "false"
            }, ClaimValueTypes.String)
        };

        string expiresInMinutesString = Environment.GetEnvironmentVariable("EXPIRES_IN_MINUTES") ?? throw new Exception("EXPIRES_IN_MINUTES not found");
        int expiresInMinutes = int.Parse(expiresInMinutesString);
        DateTime expires = DateTime.UtcNow.AddMinutes(expiresInMinutes);

        // Change symmetric key?
        string tokenKey = Environment.GetEnvironmentVariable("TOKEN_KEY") ?? throw new Exception("TOKEN_KEY not found");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
        
        string issuer = Environment.GetEnvironmentVariable("ISSUER") ?? throw new Exception("ISSUER not found");
        string audience = Environment.GetEnvironmentVariable("AUDIENCE") ?? throw new Exception("AUDIENCE not found");
        
        var token = new JwtSecurityToken(
                issuer: issuer, 
                audience: audience, 
                claims: claims,
                expires: expires,
                signingCredentials: credentials);

        string? handler = new JwtSecurityTokenHandler().WriteToken(token);

        return (handler, expires);
    }

    private async Task UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        await _userRepository.UpdateUserAsync(user, cancellationToken);
    }
}