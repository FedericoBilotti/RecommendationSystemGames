using App.Dtos.Engine;
using App.Dtos.Games.Requests;
using App.Dtos.Games.Responses;
using App.Interfaces.Engine;
using Microsoft.AspNetCore.Mvc;
using RM.Domain.Entities.Games;
using RM.Presentation.Mappers;
using RM.Presentation.Routes;

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
        
        GameResponseDto gameResponseDto = game.MapToGameResponseDto();
        return Created($"{ApiEndpoints.V1.Games.CREATE}/{game.GameId}", gameResponseDto);
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