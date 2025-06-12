using App.Auth;
using App.Dtos.Games.Requests;
using App.Dtos.Games.Responses;
using App.Interfaces.Engine;
using App.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RM.Domain.Entities.Games;
using RM.Presentation.Routes;
using RM.Presentation.Utility;

namespace RM.Presentation.Controllers.Engine;

[ApiController]
public class RatingController(IRatingUseCase ratingUseCase) : ControllerBase
{
    [Authorize]
    [HttpPut(ApiEndpoints.V1.Games.RATE)]
    public async Task<IActionResult> RateGame([FromRoute] Guid id, [FromBody] GameRateRequestDto gameRateRequestDto, CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId(); // gonna throw exception cause i don't add user GUID yet.
        bool result = await ratingUseCase.RateGameAsync(id, gameRateRequestDto.Rating, userId!.Value, cancellationToken);
        return result ? Ok() : NotFound();
    }

    [Authorize]
    [HttpDelete(ApiEndpoints.V1.Games.DELETE_RATE)]
    public async Task<IActionResult> DeleteRating([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId();
        bool result = await ratingUseCase.DeleteRatingAsync(id, userId!.Value, cancellationToken);
        return result ? Ok() : NotFound();
    }
    
    [Authorize]
    [HttpGet(ApiEndpoints.V1.Ratings.GET_USER_RATINGS)]
    public async Task<IActionResult> GetUserRatings(CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId();
        IEnumerable<GameRating> ratings = await ratingUseCase.GetUserRatingsAsync(userId!.Value, cancellationToken);
        IEnumerable<GameRatingResponseDto> response = ratings.MapToResponse();
        return Ok(response);
    }
}