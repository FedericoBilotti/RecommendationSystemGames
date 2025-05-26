using Microsoft.AspNetCore.Mvc;

namespace App.API;

[ApiController]
[Route("[controller]")]
public class RecommendationController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(Array.Empty<string>());
    }
}