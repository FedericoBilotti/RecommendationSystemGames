using App.Dtos.Authentication.Request;
using FluentValidation;

namespace App.Services.Validators.Users;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenRequestDto>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}