using App.Dtos.Engine;
using App.Dtos.Games.Requests;
using App.Dtos.Games.Responses;
using App.Interfaces.Engine;
using RM.Domain.Entities.Games;
using RM.Presentation.Mappers;
using RM.Presentation.Routes;
using Microsoft.AspNetCore.Mvc;

namespace RM.Presentation.Controllers.Engine;

[ApiController]
public class GamesController(IGamesRepository gamesRepository) : ControllerBase
{
    [HttpPost(ApiEndpoints.V1.Games.CREATE)]
    public async Task<ActionResult<GameFilterResponseDto>> Create([FromBody] CreateGameRequestDto createGameRequest, CancellationToken cancellationToken = default)
    {
        Game game = createGameRequest.MapToGame();
        bool wasCreated = await gamesRepository.CreateAsync(game, cancellationToken);

        if (!wasCreated)
        {
            return BadRequest("Game already exists");
        }
        
        GameResponseDto gameResponse = game.MapToResponse();
        return CreatedAtAction(nameof(Get), new { id = game.GameId }, gameResponse);
    }

    [HttpGet(ApiEndpoints.V1.Games.GET)]
    public async Task<ActionResult<GameFilterResponseDto>> Get([FromRoute] Guid id, CancellationToken cancellationToken = default)
    {
        Game? game = await gamesRepository.GetByIdAsync(id, cancellationToken);
        
        if (game == null)
        {
            return NotFound("Game not found");
        }
        
        GameResponseDto gameResponse = game.MapToResponse();
        return Ok(gameResponse);
    }
    
    [HttpGet(ApiEndpoints.V1.Games.GET_ALL)]
    public async Task<ActionResult<GameFilterResponseDto>> GetAll(CancellationToken cancellationToken = default)
    {
        IEnumerable<Game> game = await gamesRepository.GetAllAsync(cancellationToken);
        
        GamesResponseDto gameResponse = game.MapToResponse();
        
        return Ok(gameResponse);
    }

    [HttpPut(ApiEndpoints.V1.Games.UPDATE)]
    public async Task<ActionResult<GameFilterResponseDto>> Update([FromRoute] Guid id, [FromBody] UpdateGameRequestDto updateGameRequest, CancellationToken cancellationToken = default)
    {
        Game game = updateGameRequest.MapToGame(id);
        
        bool wasUpdated = await gamesRepository.UpdateAsync(game, cancellationToken);
        
        if (!wasUpdated)
        {
            return BadRequest("Game not found");
        }

        GameResponseDto gameResponse = game.MapToResponse();
        return Ok(gameResponse);
    }
    
    /*
    [HttpPost(ApiEndpoints.V1.Games.FILTER)]
    public async Task<ActionResult<GameFilterResponseDto>> Filter(GameFilterRequestDto requestDto)
    {
        GameFilterResponseDto? responseDto = await engineUseCase.GetGamesByFiltersAsync(requestDto);
        
        if (responseDto == null)
        {
            return NotFound("No developers found");
        }
        
        return Ok(responseDto);
    }
    
    // [Authorize]
    [HttpPost(ApiEndpoints.V1.Games.FILTER_GENRES)]
    public async Task<ActionResult<GameGenreFilterResponseDto>> FilterGenres(GameGenreFilterRequestDto gameGenresName)
    {
        GameGenreFilterResponseDto? games = await engineUseCase.GetGamesByGenreAsync(gameGenresName);
        
        if (games == null)
        {
            return NotFound("No games found");
        }
        
        return Ok(games);
    }
    
    // [Authorize]
    [HttpPost(ApiEndpoints.V1.Games.FILTER_DEVELOPERS)]
    public async Task<ActionResult<GameDevelopersFilterResponseDto>> FilterDevelopers(GameDevelopersFilterRequestDto requestDto)
    {
        GameDevelopersFilterResponseDto? developers = await engineUseCase.GetGamesByDeveloperAsync(requestDto);
        
        if (developers == null)
        {
            return NotFound("No developers found");
        }
        
        return Ok(developers);
    }
    */
}