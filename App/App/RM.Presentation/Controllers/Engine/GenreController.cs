using App.RM.Application.Dtos.Engine;
using Microsoft.AspNetCore.Mvc;

namespace App.RM.Presentation.Controllers.Engine;

[Route("api/v1/[controller]")]
[ApiController]
public class GenreController : ControllerBase
{
    // [Authorize]
    [HttpGet("Filter")]
    public IActionResult FilterGenres([FromQuery] GenreFilterDto genresName)
    {
        
        
        return Ok("Return");
    }
}