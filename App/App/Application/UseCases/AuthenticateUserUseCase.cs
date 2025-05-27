using App.Application.Dtos;
using App.Application.Interfaces;
using App.Domain.Entities;
using App.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;

namespace App.Application.UseCases;

public class AuthenticateUserUseCase(
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

        string hashedPassword = hasher.HashPassword(user, requestUserDto.Password);
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

        PasswordVerificationResult passwordVerificationResult = hasher.VerifyHashedPassword(user, user.HashedPassword, requestUserDto.Password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed) return null;

        return await tokens.CreateTokenResponse(user);
    }
}