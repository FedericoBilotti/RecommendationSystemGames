using App.Dtos.Authentication.Request;
using RM.Domain.Entities;

namespace App.Interfaces.Authentication;

public interface IValidationService
{
    Task ValidateUserAndThrowAsync(User user, CancellationToken cancellationToken);        
    Task ValidateLoginAndThrowAsync(UserLoginRequestDto userLoginRequestDto, CancellationToken cancellationToken);        
    Task ValidateTokenAndThrowAsync(RefreshTokenRequestDto refreshTokenRequestDto, CancellationToken cancellationToken);        
}