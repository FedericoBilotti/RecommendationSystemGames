using App.Dtos.Authentication.Request;
using App.Interfaces;
using Microsoft.AspNetCore.Identity;
using RM.Domain.Entities;

namespace App.Services;

public class HasherService : IHasherService
{
    private readonly IPasswordHasher<UserRegisterRequestDto> _hasherRegister;
    private readonly IPasswordHasher<User> _hasherLogin;

    public HasherService(IPasswordHasher<UserRegisterRequestDto> hasherRegister, IPasswordHasher<User> hasherLogin)
    {
        _hasherRegister = hasherRegister;
        _hasherLogin = hasherLogin;
    }
    
    public string RegisterHasher(UserRegisterRequestDto dto)
    {
        return _hasherRegister.HashPassword(dto, dto.Password);
    }
    
    public PasswordVerificationResult LoginHasher(User user, UserLoginRequestDto dto)
    {
        return _hasherLogin.VerifyHashedPassword(user, user.HashedPassword, dto.Password);
    }
}