using App.Dtos.Games.Requests;
using FluentValidation;

namespace App.Services.Validators.Games;

public class GetAllValidatorDto : AbstractValidator<GetAllGameRequestDto>
{
    public GetAllValidatorDto()
    {
        RuleFor(x => x.YearOfRelease)
                .LessThanOrEqualTo(DateTime.Now.Year);
        
        RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page must be greater than or equal to 1");
        
        RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(25)
                .WithMessage("Page size must be between 1 and 25");
    }
}