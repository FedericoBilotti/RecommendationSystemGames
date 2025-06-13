using FluentValidation;
using RM.Domain.Entities;

namespace App.Services.Validators.Games;

public class GetAllGameOptionsValidator : AbstractValidator<GetAllGameOptions>
{
    private static readonly string[] AcceptableSortFields = ["title", "yearOfRelease"];
    
    public GetAllGameOptionsValidator()
    {
        RuleFor(x => x.SortField)
                .Must(x => x == null || AcceptableSortFields.Contains(x, StringComparer.OrdinalIgnoreCase))
                .WithMessage("Sort field must be: 'title' or 'yearofrelease'");
    }
}