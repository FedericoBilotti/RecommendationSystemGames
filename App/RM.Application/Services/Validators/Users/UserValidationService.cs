using App.Dtos.Authentication.Request;
using App.Interfaces.Authentication;
using FluentValidation;
using RM.Domain.Entities;

namespace App.Services.Validators.Users;

public class UserValidationService(IValidator<UserRegisterRequestDto> userValidator, IValidator<UserLoginRequestDto> userLoginValidator, IValidator<RefreshTokenRequestDto> refreshTokenValidator) : IUserValidationService
{
    public Task ValidateUserAndThrowAsync(UserRegisterRequestDto userRegisterRequestDto, CancellationToken cancellationToken)
    {
        return userValidator.ValidateAndThrowAsync(userRegisterRequestDto, cancellationToken);
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