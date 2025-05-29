using App.RM.Application.Dtos.Engine;
using App.RM.Application.Interfaces.Engine;
using Microsoft.AspNetCore.Mvc;

namespace App.RM.Presentation.Controllers.Engine;

[Route("api/v1/[controller]")]
[ApiController]
public class GenreController(IEngineUseCase engineUseCase) : ControllerBase
{
    // [Authorize]
    [HttpGet("Filter")]
    public async Task<ActionResult<GameGenreFilterResponseDto>> FilterGenres(GameGenreFilterRequestDto gameGenresName)
    {
        GameGenreFilterResponseDto? games = await engineUseCase.GetGamesByGenreAsync(gameGenresName);
        
        if (games == null)
        {
            return NotFound("No games found");
        }
        
        return Ok(games);
    }
}