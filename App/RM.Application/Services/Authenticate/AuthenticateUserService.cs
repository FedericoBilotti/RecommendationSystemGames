using App.Dtos.Authentication.Request;
using App.Dtos.Authentication.Response;
using App.Interfaces;
using App.Interfaces.Authentication;
using Microsoft.AspNetCore.Identity;
using RM.Domain.Entities;

namespace App.Services.Authenticate;

public class AuthenticateUserService(IUserRepository userRepository, IPasswordHasher<User> hasher, ITokenService tokens) : IAuthService
{
    // It's going to be the user instead of the dto
    public async Task<User?> RegisterAsync(UserRequestDto requestUserRequestDto)
    {
        // User user = requestUserRequestDto.MapToUserExists();
        User user = new User();
        
        bool isValid = await userRepository.GetUserByUsername(requestUserRequestDto.Username) != null;
        
        if (isValid) return null; // Throw an error

        // user.MapToUserRegister(hasher, requestUserRequestDto.Password);

        // context.Users.Add(user); // Debería hacerlo el repositorio.
        // await context.SaveChangesAsync();

        return user;
    }

    public async Task<TokenResponseDto?> LoginAsync(UserRequestDto requestUserRequestDto)
    {
        // User? user = await userRepository.GetUserByUsername(requestUserRequestDto.MapToUserExists());
        User user = new User();
        
        if (user == null) return null;

        PasswordVerificationResult passwordVerificationResult = hasher.VerifyHashedPassword(user, user.HashedPassword, requestUserRequestDto.Password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed) return null;

        return await tokens.CreateTokenResponse(user);
    }
}