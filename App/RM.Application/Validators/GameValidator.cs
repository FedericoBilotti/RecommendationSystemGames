using App.Interfaces.Engine;
using FluentValidation;
using RM.Domain.Entities.Games;

namespace App.Validators;

public class GameValidator : AbstractValidator<Game>
{
    private readonly IGamesRepository _gameRepository;

    public GameValidator(IGamesRepository gameRepository)
    {
        _gameRepository = gameRepository;
        
        RuleFor(x => x.GameId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Genres).NotEmpty();
        
        RuleFor(x => x.Slug)
                .MustAsync(ValidateSlug)
                .WithMessage("Slug already exists");
        
        RuleFor(x => x.YearOfRelease)
                .LessThanOrEqualTo(DateTime.UtcNow.Year)
                .GreaterThanOrEqualTo(1952);
    }

    private async Task<bool> ValidateSlug(Game game, string slug, CancellationToken cancellationToken = default)
    {
        Game? existingGame = await _gameRepository.GetBySlugAsync(slug, cancellationToken: cancellationToken);

        if (existingGame != null)
        {
            return existingGame.GameId == game.GameId;
        }

        return existingGame == null;
    }
}