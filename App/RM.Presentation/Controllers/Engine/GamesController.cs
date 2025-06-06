using App.Dtos.Games.Requests;
using App.Dtos.Games.Responses;
using App.Interfaces.Engine;
using RM.Domain.Entities.Games;
using RM.Presentation.Mappers;
using RM.Presentation.Routes;
using Microsoft.AspNetCore.Mvc;

namespace RM.Presentation.Controllers.Engine;

[ApiController]
public class GamesController(IGameService gameService) : ControllerBase
{
    [HttpPost(ApiEndpoints.V1.Games.CREATE)]
    public async Task<ActionResult<GameResponseDto>> Create([FromBody] CreateGameRequestDto createGameRequest, CancellationToken cancellationToken = default)
    {
        Game game = createGameRequest.MapToGame();
        bool wasCreated = await gameService.CreateAsync(game, cancellationToken);

        if (!wasCreated)
        {
            return BadRequest("Game already exists");
        }

        GameResponseDto gameResponse = game.MapToResponse();
        return CreatedAtAction(nameof(Get), new { idOrSlug = game.GameId }, gameResponse);
    }

    [HttpGet(ApiEndpoints.V1.Games.GET)]
    public async Task<ActionResult<GameResponseDto>> Get([FromRoute] string idOrSlug, CancellationToken cancellationToken = default)
    {
        Game? game = Guid.TryParse(idOrSlug, out Guid gameId) 
                ? await gameService.GetByIdAsync(gameId, cancellationToken) 
                : await gameService.GetBySlugAsync(idOrSlug, cancellationToken);

        if (game == null)
        {
            return NotFound("Game not found");
        }

        GameResponseDto gameResponse = game.MapToResponse();
        return Ok(gameResponse);
    }

    [HttpGet(ApiEndpoints.V1.Games.GET_ALL)]
    public async Task<ActionResult<GamesResponseDto>> GetAll(CancellationToken cancellationToken = default)
    {
        IEnumerable<Game> game = await gameService.GetAllAsync(cancellationToken);

        GamesResponseDto gameResponse = game.MapToResponse();

        return Ok(gameResponse);
    }

    [HttpPut(ApiEndpoints.V1.Games.UPDATE)]
    public async Task<ActionResult<GameResponseDto>> Update([FromRoute] Guid id, [FromBody] UpdateGameRequestDto updateGameRequest, CancellationToken cancellationToken = default)
    {
        Game game = updateGameRequest.MapToGame(id);
        Game? wasUpdated = await gameService.UpdateAsync(game, cancellationToken);

        if (wasUpdated == null)
        {
            return NotFound("Game not found");
        }

        GameResponseDto gameResponse = wasUpdated.MapToResponse();
        return Ok(gameResponse);
    }

    [HttpDelete(ApiEndpoints.V1.Games.DELETE)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        bool wasDeleted = await gameService.DeleteByIdAsync(id, cancellationToken);

        if (!wasDeleted)
        {
            return NotFound("Game not found");
        }

        return Ok();
    }
}