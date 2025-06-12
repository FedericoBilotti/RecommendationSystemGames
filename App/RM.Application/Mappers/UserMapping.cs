using App.Dtos.Authentication.Request;
using App.Dtos.Authentication.Response;
using Microsoft.AspNetCore.Identity;
using RM.Domain.Entities;

namespace App.Mappers;

public static class UserMapping
{
    public static User MapToUser(this UserRegisterRequestDto userLoginRequestDto, string password)
    {
        return new User
        {
            UserId = Guid.NewGuid(),
            Username = userLoginRequestDto.Username,
            Email = userLoginRequestDto.Email,
            HashedPassword = password,
            Role = "User",
            TrustedUser = false
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