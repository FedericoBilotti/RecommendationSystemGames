using App.App.Domain.Entities;
using App.Application.Interfaces;
using App.Infrastructure.Models.Dtos;
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
        TokenResponeDto? tokenResult = await authService.LoginAsync(requestUserDto);
        
        if (tokenResult == null)
            return BadRequest("User not found or the password is wrong");
        
        return Ok(tokenResult);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<TokenResponeDto>> RefreshToken(RefreshTokenRequestDto requestRefreshTokenDto)
    {
        TokenResponeDto? result = await authService.RefreshTokenAsync(requestRefreshTokenDto);
        
        if (result?.AccessToken == null || result?.RefreshToken == null)
            return Unauthorized("Refresh token is invalid");
        
        return Ok(result);
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