using App.RM.Application.Dtos.Authentication;
using App.RM.Domain.Entities;

namespace App.RM.Application.Interfaces.Authentication;

public interface IAuthService
{
    Task<User?> RegisterAsync(UserRequestDto requestUserRequestDto);
    Task<TokenResponseDto?> LoginAsync(UserRequestDto requestUserRequestDto);
}