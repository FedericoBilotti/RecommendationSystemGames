using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using App.Application.Interfaces;
using App.Domain.Entities;
using App.Infrastructure.Models.User;
using App.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace App.Application.Services;

public class AuthenticateUserService(AppDbContext context, IConfiguration configuration) : IAuthService
{
    public async Task<User?> RegisterAsync(UserDto requestUserDto)
    {
        if (await SearchUser(requestUserDto) != null)
            return null;

        var user = new User();
        
        string hashedPassword = new PasswordHasher<User>().HashPassword(user, requestUserDto.Password);
        user.Username = requestUserDto.Username;
        user.HashedPassword = hashedPassword;
        
        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user;
    }

    public async Task<string?> LoginAsync(UserDto requestUserDto)
    {
        var user = await SearchUser(requestUserDto);

        if (user == null) 
            return null;

        PasswordVerificationResult passwordVerificationResult = new PasswordHasher<User>().VerifyHashedPassword(user, user.HashedPassword, requestUserDto.Password);
        
        if (passwordVerificationResult == PasswordVerificationResult.Failed)
            return null;
        
        return GenerateToken(user);
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
        var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }

    private async Task<User?> SearchUser(UserDto requestUserDto)
    {
        string usernameLower = requestUserDto.Username.ToLower(); 
        return await context.Users.FirstOrDefaultAsync(u => u.Username == usernameLower);
    }
}