using App.Dtos.Authentication.Request;
using App.Interfaces.Authentication;
using FluentValidation;
using RM.Domain.Entities;

namespace App.Services.Validators.Users;

public class UserValidationService(IValidator<User> userValidator, IValidator<UserLoginRequestDto> userLoginValidator, IValidator<RefreshTokenRequestDto> refreshTokenValidator) : IValidationService
{
    public Task ValidateUserAndThrowAsync(User user, CancellationToken cancellationToken)
    {
        return userValidator.ValidateAndThrowAsync(user, cancellationToken);
    }

    public Task ValidateLoginAndThrowAsync(UserLoginRequestDto userLoginRequestDto, CancellationToken cancellationToken)
    {
        return userLoginValidator.ValidateAndThrowAsync(userLoginRequestDto, cancellationToken);
    }

    public Task ValidateTokenAndThrowAsync(RefreshTokenRequestDto refreshTokenRequestDto, CancellationToken cancellationToken)
    {
        return refreshTokenValidator.ValidateAndThrowAsync(refreshTokenRequestDto, cancellationToken);
    }
}