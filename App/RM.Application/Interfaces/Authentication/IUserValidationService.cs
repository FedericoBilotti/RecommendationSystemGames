using App.Dtos.Authentication.Request;
using RM.Domain.Entities;

namespace App.Interfaces.Authentication;

public interface IUserValidationService
{
    Task ValidateUserAndThrowAsync(UserRegisterRequestDto userRegisterRequestDto, CancellationToken cancellationToken);        
    Task ValidateLoginAndThrowAsync(UserLoginRequestDto userLoginRequestDto, CancellationToken cancellationToken);        
    Task ValidateTokenAndThrowAsync(RefreshTokenRequestDto refreshTokenRequestDto, CancellationToken cancellationToken);        
}