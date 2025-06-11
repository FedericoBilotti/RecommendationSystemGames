using App.Dtos.Authentication.Request;
using App.Interfaces;
using Microsoft.AspNetCore.Identity;
using RM.Domain.Entities;

namespace App.Services;

public class HasherService(IPasswordHasher<UserRegisterRequestDto> hasher, IPasswordHasher<User> hasherLogin) : IHasherService
{
    public string RegisterHasher(UserRegisterRequestDto dto)
    {
        return hasher.HashPassword(dto, dto.Password);
    }
    
    public PasswordVerificationResult LoginHasher(User user, UserLoginRequestDto dto)
    {
        return hasherLogin.VerifyHashedPassword(user, user.HashedPassword, dto.Password);
    }
}