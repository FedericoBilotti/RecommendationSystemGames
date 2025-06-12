using App.Dtos.Games.Requests;
using FluentValidation;

namespace App.Services.Validators.Games;

public class GetAllValidator : AbstractValidator<GetAllGameRequest>
{
    public GetAllValidator()
    {
        RuleFor(x => x.YearOfRelease)
                .LessThanOrEqualTo(DateTime.Now.Year);
    }    
}