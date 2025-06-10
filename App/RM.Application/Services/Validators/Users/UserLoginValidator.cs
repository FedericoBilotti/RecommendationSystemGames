using App.Dtos.Authentication.Request;
using FluentValidation;

namespace App.Services.Validators.Users;

public class UserLoginValidator : AbstractValidator<UserLoginRequestDto>
{
    public UserLoginValidator()
    {
        RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.Email) || !string.IsNullOrWhiteSpace(x.Username))
                .WithMessage("Either email or username must be provided");

        RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required");
    }
}