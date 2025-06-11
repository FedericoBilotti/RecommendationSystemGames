using App.Dtos.Authentication.Request;
using Microsoft.AspNetCore.Identity;
using RM.Domain.Entities;

namespace App.Interfaces;

public interface IHasherService
{
    string RegisterHasher(UserRegisterRequestDto hasher);
    PasswordVerificationResult LoginHasher(User user, UserLoginRequestDto dto);
}