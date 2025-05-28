using App.RM.Application.Dtos;
using App.RM.Domain.Entities;

namespace App.RM.Application.Interfaces;

public interface IAuthService
{
    Task<User?> RegisterAsync(UserRequestDto requestUserRequestDto);
    Task<TokenResponseDto?> LoginAsync(UserRequestDto requestUserRequestDto);
}