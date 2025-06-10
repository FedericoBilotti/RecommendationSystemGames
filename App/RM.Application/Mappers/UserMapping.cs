using App.Dtos.Authentication.Request;
using App.Dtos.Authentication.Response;
using Microsoft.AspNetCore.Identity;
using RM.Domain.Entities;

namespace App.Mappers;

public static class UserMapping
{
    public static User MapToUser(this UserRegisterRequestDto userLoginRequestDto, IPasswordHasher<UserRegisterRequestDto> hasher)
    {
        return new User
        {
            UserId = Guid.NewGuid(),
            Username = userLoginRequestDto.Username,
            Email = userLoginRequestDto.Email,
            HashedPassword = hasher.HashPassword(userLoginRequestDto, userLoginRequestDto.Password)
        };
    }

    public static User MapToUser(this UserLoginRequestDto userLogin, Guid userId)
    {
        return new User
        {
            UserId = userId,
            Username = userLogin.Username!,
            Email = userLogin.Email!,
            HashedPassword = userLogin.Password
        };
    }

    public static UserResponseDto MapToUserResponse(this User user)
    {
        return new UserResponseDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
        };
    }
}