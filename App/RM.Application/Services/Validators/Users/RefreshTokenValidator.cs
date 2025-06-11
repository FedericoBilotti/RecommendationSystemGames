using App.Dtos.Authentication.Request;
using App.Interfaces;
using FluentValidation;
using RM.Domain.Entities;

namespace App.Services.Validators.Users;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenRequestDto>
{

    public RefreshTokenValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}