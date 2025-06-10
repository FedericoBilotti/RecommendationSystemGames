using App.Dtos.Authentication.Request;
using App.Dtos.Authentication.Response;
using RM.Domain.Entities;

namespace App.Interfaces.Authentication;

public interface IAuthService
{
    Task<User?> RegisterAsync(User requestUserRequestDto);
    Task<TokenResponseDto?> LoginAsync(UserRequestDto requestUserRequestDto);
}