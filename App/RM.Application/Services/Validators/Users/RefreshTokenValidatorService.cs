using App.Dtos.Authentication.Request;
using FluentValidation;

namespace App.Services.Validators.Users;

public class RefreshTokenValidatorService : AbstractValidator<RefreshTokenRequestDto>
{
    public RefreshTokenValidatorService()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}