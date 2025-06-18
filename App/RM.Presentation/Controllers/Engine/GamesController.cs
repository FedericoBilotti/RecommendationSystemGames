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
    [ProducesResponseType(typeof(GameResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GameResponseDto>> Create([FromBody] CreateGameRequestDto createGameRequest, CancellationToken cancellationToken = default)
    {
        GameResponseDto? gameResponseDto = await gameUseCase.CreateAsync(createGameRequest, cancellationToken);

        if (gameResponseDto == null)
        {
            return BadRequest("Game already exists");
        }
        
        return CreatedAtAction(nameof(Get), new { idOrSlug = gameResponseDto.GameId }, gameResponseDto); 
    }

    [Authorize]
    [HttpGet(ApiEndpoints.V1.Games.GET)]
    [ProducesResponseType(typeof(GameResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    [Authorize]
    [HttpGet(ApiEndpoints.V1.Games.GET_ALL)]
    [ProducesResponseType(typeof(GameResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<GamesResponseDto>> GetAll([FromQuery] GetAllGameRequestDto getAllGameRequestDto, CancellationToken cancellationToken = default)
    {
        Guid? userId = HttpContext.GetUserId();
        GamesResponseDto gamesResponseDto = await gameUseCase.GetAllAsync(getAllGameRequestDto, userId, cancellationToken);
        return Ok(gamesResponseDto);
    }

    [Authorize(AuthConstants.TRUSTED_ROLE)]
    [HttpPut(ApiEndpoints.V1.Games.UPDATE)]
    [ProducesResponseType(typeof(GameResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameResponseDto>> Update([FromBody] UpdateGameRequestDto updateGameRequest, [FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
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
    [ProducesResponseType(typeof(GameResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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