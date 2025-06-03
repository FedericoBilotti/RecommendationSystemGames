using App.Interfaces.Authentication;
using App.RM.Application.Dtos.Authentication;
using App.RM.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using RM.Domain.Entities;

namespace App.Services.Authenticate;

public class AuthenticateUserService(
        IUserRepository context, // Pedir el user repository
        IUserRepository userRepository, 
        IPasswordHasher<User> hasher, 
        ITokenService tokens) 
        : IAuthService
{
    public async Task<User?> RegisterAsync(UserRequestDto requestUserRequestDto)
    {
        if (await userRepository.GetUserByUsername(requestUserRequestDto) != null) return null;

        var user = new User();

        string hashedPassword = hasher.HashPassword(user, requestUserRequestDto.Password);
        user.Username = requestUserRequestDto.Username;
        user.HashedPassword = hashedPassword;
        user.Email = requestUserRequestDto.Email;

        // context.Users.Add(user); // Debería hacerlo el repositorio.
        // await context.SaveChangesAsync();

        return user;
    }

    public async Task<TokenResponseDto?> LoginAsync(UserRequestDto requestUserRequestDto)
    {
        User? user = await userRepository.GetUserByUsername(requestUserRequestDto);

        if (user == null) return null;

        PasswordVerificationResult passwordVerificationResult = hasher.VerifyHashedPassword(user, user.HashedPassword, requestUserRequestDto.Password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed) return null;

        return await tokens.CreateTokenResponse(user);
    }
}