using App.Dtos.Games.Requests;
using App.Dtos.Games.Responses;
using App.Interfaces.Engine;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RM.Domain.Entities.Games;
using RM.Presentation.Auth;
using RM.Presentation.Mappers;
using RM.Presentation.Routes;

namespace RM.Presentation.Controllers.Engine;

[ApiController]
public class RatingController(IRatingService ratingService) : ControllerBase
{
    [Authorize]
    [HttpPut(ApiEndpoints.V1.Games.RATE)]
    public async Task<IActionResult> RateGame([FromRoute] Guid gameId, [FromBody] GameRateRequestDto gameRateRequestDto, CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId(); // gonna throw exception cause i don't add user GUID yet.
        bool result = await ratingService.RateGameAsync(gameId, gameRateRequestDto.Rating, userId!.Value, cancellationToken);
        return result ? Ok() : NotFound();
    }

    [Authorize]
    [HttpDelete(ApiEndpoints.V1.Games.DELETE_RATE)]
    public async Task<IActionResult> DeleteRating([FromRoute] Guid gameId, CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId();
        bool result = await ratingService.DeleteRatingAsync(gameId, userId!.Value, cancellationToken);
        return result ? Ok() : NotFound();
    }
    
    [Authorize]
    [HttpGet(ApiEndpoints.V1.Ratings.GET_USER_RATINGS)]
    public async Task<IActionResult> GetUserRatings(CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId();
        IEnumerable<GameRating> ratings = await ratingService.GetUserRatingsAsync(userId!.Value, cancellationToken);
        IEnumerable<GameRatingResponseDto> response = ratings.MapToResponse();
        return Ok(response);
    }
}