using App.Interfaces;
using FluentValidation;
using RM.Domain.Entities;

namespace App.Services.Validators;

public class UserValidatorService : AbstractValidator<User>
{
    private readonly IUserRepository _userRepository;
    
    public UserValidatorService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
        
        RuleFor(x => x.UserId).NotEmpty();
        
        RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MustAsync(IsEmailUnique)
                .WithMessage("Email already exists");
        
        RuleFor(x => x.Username)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(50)
                .Matches("^[a-z0-9]+$")
                .WithMessage("Username can only contain lowercase letters and numbers")
                .Must(x => x == x.ToLower())
                .WithMessage("Username must be lowercase")
                .MustAsync(IsUsernameUnique)
                .WithMessage("Username already exists");
        
        RuleFor(x => x.HashedPassword)
                .NotEmpty()
                .MinimumLength(5)
                .WithMessage("The password must be at least 5 characters")
                .MaximumLength(100)
                .WithMessage("The password must be less than 100 characters");
    }
    
    private async Task<bool> IsUsernameUnique(User user, string username, CancellationToken cancellationToken = default)
    {
        User? existingUser = await _userRepository.GetUserByUsername(username, cancellationToken);

        if (existingUser != null)
        {
            return existingUser.UserId == user.UserId;
        }

        return existingUser == null;
    }

    private async Task<bool> IsEmailUnique(User user, string email, CancellationToken cancellationToken = default)
    {
        User? existingUser = await _userRepository.GetUserByEmail(email, cancellationToken);

        if (existingUser != null)
        {
            return existingUser.UserId == user.UserId;
        }

        return existingUser == null;
    }
}