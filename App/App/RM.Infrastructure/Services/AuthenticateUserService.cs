using App.RM.Application.Dtos.Authentication;
using App.RM.Application.Interfaces;
using App.RM.Application.Interfaces.Authentication;
using App.RM.Domain.Entities;
using App.RM.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;

namespace App.RM.Infrastructure.Services;

public class AuthenticateUserService(
        AppDbContext context, 
        IUserRepository userRepository, 
        IPasswordHasher<User> hasher, 
        ITokenService tokens) 
        : IAuthService
{
    public async Task<User?> RegisterAsync(UserRequestDto requestUserRequestDto)
    {
        if (await userRepository.GetUserByUsername(context, requestUserRequestDto) != null) return null;

        var user = new User();

        string hashedPassword = hasher.HashPassword(user, requestUserRequestDto.Password);
        user.Username = requestUserRequestDto.Username;
        user.HashedPassword = hashedPassword;

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user;
    }

    public async Task<TokenResponseDto?> LoginAsync(UserRequestDto requestUserRequestDto)
    {
        User? user = await userRepository.GetUserByUsername(context, requestUserRequestDto);

        if (user == null) return null;

        PasswordVerificationResult passwordVerificationResult = hasher.VerifyHashedPassword(user, user.HashedPassword, requestUserRequestDto.Password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed) return null;

        return await tokens.CreateTokenResponse(user);
    }
}