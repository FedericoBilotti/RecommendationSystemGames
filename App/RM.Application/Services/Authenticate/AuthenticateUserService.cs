using App.Dtos.Authentication;
using App.Interfaces;
using App.Interfaces.Authentication;
using App.Mappers;
using Microsoft.AspNetCore.Identity;
using RM.Domain.Entities;

namespace App.Services.Authenticate;

public class AuthenticateUserService(IUserRepository userRepository, IPasswordHasher<User> hasher, ITokenService tokens) : IAuthService
{
    public async Task<User?> RegisterAsync(UserRequestDto requestUserRequestDto)
    {
        User user = requestUserRequestDto.MapToUserExists();
        
        bool isValid = await userRepository.GetUserByUsername(user) != null;
        
        if (isValid) return null; // Throw an error

        user.MapToUserRegister(hasher, requestUserRequestDto.Password);

        // context.Users.Add(user); // Debería hacerlo el repositorio.
        // await context.SaveChangesAsync();

        return user;
    }

    public async Task<TokenResponseDto?> LoginAsync(UserRequestDto requestUserRequestDto)
    {
        User? user = await userRepository.GetUserByUsername(requestUserRequestDto.MapToUserExists());

        if (user == null) return null;

        PasswordVerificationResult passwordVerificationResult = hasher.VerifyHashedPassword(user, user.HashedPassword, requestUserRequestDto.Password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed) return null;

        return await tokens.CreateTokenResponse(user);
    }
}