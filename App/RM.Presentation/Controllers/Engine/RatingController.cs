using App.Dtos.Games.Requests;
using App.Interfaces.Engine;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RM.Presentation.Auth;
using RM.Presentation.Routes;

namespace RM.Presentation.Controllers.Engine;

[ApiController]
public class RatingController(IRatingService ratingService) : ControllerBase
{
    // [Authorize]
    [HttpPut(ApiEndpoints.V1.Games.RATE)]
    public async Task<IActionResult> RateGame([FromRoute] Guid gameId, [FromBody] RateGameRequestDto rateGameRequestDto, CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId(); // gonna throw exception cause i don't add user GUID yet.
        bool result = await ratingService.RateGameAsync(gameId, rateGameRequestDto.Rating, userId!.Value, cancellationToken);
        return result ? Ok() : NotFound();
    }
}