using App.Dtos.Authentication.Request;
using App.Dtos.Authentication.Response;
using Microsoft.AspNetCore.Identity;
using RM.Domain.Entities;

namespace App.Mappers;

public static class UserMapping
{
    public static User MapToNewUser(this UserRequestDto userRequestDto)
    {
        return new User
        {
            UserId = Guid.NewGuid(),
            Username = userRequestDto.Username,
            Email = userRequestDto.Email,
        };
    }
    
    public static User MapToUser(this UserRequestDto user, IPasswordHasher<UserRequestDto> hasher)
    {
        return new User
        {
            Username = user.Username,
            Email = user.Email,
            HashedPassword = hasher.HashPassword(user, user.Password)
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