using App.Dtos.Games.Requests;
using App.Dtos.Games.Responses;
using App.Interfaces.Engine;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RM.Presentation.Routes;
using RM.Presentation.Utility;

namespace RM.Presentation.Controllers.Engine;

[ApiController]
public class RatingController(IRatingUseCase ratingUseCase) : ControllerBase
{
    [Authorize]
    [HttpPut(ApiEndpoints.V1.Games.RATE)]
    [ProducesResponseType(typeof(GameResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RateGame([FromRoute] Guid id, [FromBody] GameRateRequestDto gameRateRequestDto, CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId();
        bool result = await ratingUseCase.RateGameAsync(id, gameRateRequestDto.Rating, userId!.Value, cancellationToken);
        return result ? Ok() : NotFound("Game not found");
    }

    [Authorize]
    [HttpDelete(ApiEndpoints.V1.Games.DELETE_RATE)]
    [ProducesResponseType(typeof(GameResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRating([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId();
        bool result = await ratingUseCase.DeleteRatingAsync(id, userId!.Value, cancellationToken);
        return result ? Ok() : NotFound("Game not found");
    }
    
    [Authorize]
    [HttpGet(ApiEndpoints.V1.Ratings.GET_USER_RATINGS)]
    [ProducesResponseType(typeof(GameResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserRatings(CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId();
        IEnumerable<GameRatingResponseDto> ratings = await ratingUseCase.GetUserRatingsAsync(userId!.Value, cancellationToken);
        return Ok(ratings);
    }
}