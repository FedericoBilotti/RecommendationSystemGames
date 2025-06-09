using App.Dtos.Authentication;
using App.Dtos.Authentication.Request;
using Microsoft.AspNetCore.Identity;
using RM.Domain.Entities;

namespace RM.Presentation.Mappers;

public static class UserMapping
{
    // public static User MapToUserRegister(this User user, IPasswordHasher<User> hasher, string password)
    // {
    //     return new User
    //     {
    //         Username = user.Username,
    //         Email = user.Email,
    //         HashedPassword = hasher.HashPassword(user, password)
    //     };
    // }
    //
    // public static User MapToUserExists(this UserRequestDto userRequestDto)
    // {
    //     return new User
    //     {
    //         UserId = Guid.NewGuid(),
    //         Username = userRequestDto.Username,
    //         Email = userRequestDto.Email,
    //     };
    // }
}