using App.RM.Application.Dtos.Engine;
using App.RM.Application.Dtos.Engine.Developers;
using App.RM.Application.Dtos.Engine.Genre;
using App.RM.Application.Interfaces.Engine;
using Microsoft.AspNetCore.Mvc;

namespace App.RM.Presentation.Controllers.Engine;

[Route("api/v1/[controller]")]
[ApiController]
public class GamesController(IEngineUseCase engineUseCase) : ControllerBase
{
    [HttpPost("Filter")]
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
    [HttpPost("Genre/Filter")]
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
    [HttpPost("Developers/Filter")]
    public async Task<ActionResult<GameDevelopersFilterResponseDto>> FilterDevelopers(GameDevelopersFilterRequestDto requestDto)
    {
        GameDevelopersFilterResponseDto? developers = await engineUseCase.GetGamesByDeveloperAsync(requestDto);
        
        if (developers == null)
        {
            return NotFound("No developers found");
        }
        
        return Ok(developers);
    }


}