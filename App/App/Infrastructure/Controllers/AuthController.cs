using App.Application.Interfaces;
using App.Domain.Entities;
using App.Infrastructure.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Infrastructure.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(UserDto requestUserDto)
    {
        User? user = await authService.RegisterAsync(requestUserDto);
        
        if (user == null)
            return BadRequest("User already exists");
        
        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(UserDto requestUserDto)
    {
        string? token = await authService.LoginAsync(requestUserDto);
        
        if (token == null)
            return BadRequest("User not found or the password is wrong");
        
        return Ok(token);
    }

    [Authorize]
    [HttpGet("Authenticate")]
    public IActionResult AuthenticatedOnlyEndpoint()
    {
        return Ok("You are authenticated");
    }
    
    
    [Authorize(Roles = "Admin")]
    [HttpGet("admin-only")]
    public IActionResult AuthenticatedAdmin()
    {
        return Ok("You are authenticated as an admin");
    }
}