using App.Dtos.Authentication;
using Microsoft.AspNetCore.Identity;
using RM.Domain.Entities;

namespace App.Mappers;

public static class ContractMapping
{
    public static User MapToUserRegister(this User user, IPasswordHasher<User> hasher, string password)
    {
        return new User
        {
            Username = user.Username,
            Email = user.Email,
            HashedPassword = hasher.HashPassword(user, password)
        };
    }
    
    public static User MapToUserExists(this UserRequestDto userRequestDto)
    {
        return new User
        {
            Username = userRequestDto.Username,
            Email = userRequestDto.Email
        };
    }
}