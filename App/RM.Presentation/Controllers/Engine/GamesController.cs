using App.Auth;
using App.Dtos.Games.Requests;
using App.Dtos.Games.Responses;
using App.Interfaces.Engine;
using App.Mappers;
using Microsoft.AspNetCore.Authorization;
using RM.Domain.Entities.Games;
using RM.Presentation.Routes;
using Microsoft.AspNetCore.Mvc;
using RM.Presentation.Utility;

namespace RM.Presentation.Controllers.Engine;

[ApiController]
public class GamesController(IGameUseCase gameUseCase) : ControllerBase
{
    [Authorize(AuthConstants.TRUSTED_ROLE)]
    [HttpPost(ApiEndpoints.V1.Games.CREATE)]
    public async Task<ActionResult<GameResponseDto>> Create([FromBody] CreateGameRequestDto createGameRequest, CancellationToken cancellationToken = default)
    {
        GameResponseDto? gameResponseDto = await gameUseCase.CreateAsync(createGameRequest, cancellationToken);

        if (gameResponseDto == null)
        {
            return BadRequest("Game already exists");
        }
        
        return CreatedAtAction(nameof(Get), new { idOrSlug = gameResponseDto.GameId }, gameResponseDto);
    }

    [HttpGet(ApiEndpoints.V1.Games.GET)]
    public async Task<ActionResult<GameResponseDto>> Get([FromRoute] string idOrSlug, CancellationToken cancellationToken = default)
    {
        Guid? userId = HttpContext.GetUserId();
        Game? game = Guid.TryParse(idOrSlug, out Guid gameId) 
                ? await gameUseCase.GetByIdAsync(gameId, userId, cancellationToken) 
                : await gameUseCase.GetBySlugAsync(idOrSlug, userId, cancellationToken);

        if (game == null)
        {
            return NotFound("Game not found");
        }

        GameResponseDto gameResponse = game.MapToResponse();
        return Ok(gameResponse);
    }

    [HttpGet(ApiEndpoints.V1.Games.GET_ALL)]
    public async Task<ActionResult<GamesResponseDto>> GetAll([FromQuery] GetAllGameRequest getAllGameRequest, CancellationToken cancellationToken = default)
    {
        Guid? userId = HttpContext.GetUserId();
        IEnumerable<Game> game = await gameUseCase.GetAllAsync(getAllGameRequest, userId, cancellationToken);

        GamesResponseDto gameResponse = game.MapToResponse();

        return Ok(gameResponse);
    }

    [Authorize(AuthConstants.TRUSTED_ROLE)]
    [HttpPut(ApiEndpoints.V1.Games.UPDATE)]
    public async Task<ActionResult<GameResponseDto>> Update([FromBody] UpdateGameRequestDto updateGameRequest, [FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        // Maybe there is a problem if i dont map before
        // Game game = updateGameRequest.MapToGame(id);
        Guid? userId = HttpContext.GetUserId();
        GameResponseDto? gameResponseDto = await gameUseCase.UpdateAsync(updateGameRequest, id, userId, cancellationToken);

        if (gameResponseDto == null)
        {
            return NotFound("Game not found");
        }

        return Ok(gameResponseDto);
    }

    [Authorize(AuthConstants.ADMIN_ROLE)]
    [HttpDelete(ApiEndpoints.V1.Games.DELETE)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        bool wasDeleted = await gameUseCase.DeleteByIdAsync(id, cancellationToken);

        if (!wasDeleted)
        {
            return NotFound("Game not found");
        }

        return Ok();
    }
}