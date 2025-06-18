using App.Dtos.Authentication.Request;
using App.Interfaces;
using FluentValidation;
using RM.Domain.Entities;

namespace App.Services.Validators.Users;

public class UserRegisterValidator : AbstractValidator<UserRegisterRequestDto>
{
    private readonly IUserRepository _userRepository;
    
    public UserRegisterValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;
        
        RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MustAsync(IsEmailUnique)
                .WithMessage("Email already exists");
        
        RuleFor(x => x.Username)
                .NotEmpty()
                .MinimumLength(3)
                .WithMessage("Username must be at least 3 characters")
                .MaximumLength(50)
                .WithMessage("Username must be less than 50 characters")
                .Matches("^[a-zA-Z0-9]+$")
                .WithMessage("Username can only contain lowercase letters and numbers")
                .MustAsync(IsUsernameUnique)
                .WithMessage("Username already exists");
        
        RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(5)
                .WithMessage("The password must be at least 5 characters")
                .MaximumLength(100)
                .WithMessage("The password must be less than 100 characters");
    }

    private async Task<bool> IsUsernameUnique(UserRegisterRequestDto userRegisterRequestDto, string username, CancellationToken cancellationToken = default)
    {
        User? existingUser = await _userRepository.GetUserByUsername(username, cancellationToken);

        return existingUser == null;
    }

    private async Task<bool> IsEmailUnique(UserRegisterRequestDto userRegisterRequestDto, string email, CancellationToken cancellationToken = default)
    {
        User? existingUser = await _userRepository.GetUserByEmail(email, cancellationToken);

        return existingUser == null;
    }
}