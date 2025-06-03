using App.Dtos.Authentication;
using RM.Domain.Entities;

namespace App.Interfaces.Authentication;

public interface IAuthService
{
    Task<User?> RegisterAsync(UserRequestDto requestUserRequestDto);
    Task<TokenResponseDto?> LoginAsync(UserRequestDto requestUserRequestDto);
}