using App.Dtos.Authentication;
using App.Dtos.Authentication.Request;
using App.Dtos.Authentication.Response;
using App.Interfaces.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RM.Domain.Entities;
using RM.Presentation.Routes;

namespace RM.Presentation.Controllers.Authentication;

[ApiController]
public class AuthController(IAuthenticateUserUseCase authUseCase) : ControllerBase
{
    [HttpPost(AuthEndpoints.Auth.REGISTER)]
    public async Task<ActionResult<User>> Register(UserRequestDto requestUserRequestDto)
    {
        User? user = await authUseCase.RegisterAsync(requestUserRequestDto);

        if (user == null)
        {
            return BadRequest("User already exists");
        }

        return Created($"{AuthEndpoints.Auth.REGISTER}/{user.UserId}", user);
    }

    [HttpPost(AuthEndpoints.Auth.LOGIN)]
    public async Task<ActionResult<string>> Login(UserRequestDto requestUserRequestDto)
    {
        TokenResponseDto? tokenResult = await authUseCase.LoginAsync(requestUserRequestDto);

        if (tokenResult == null)
        {
            return BadRequest("User not found or the password is wrong");
        }

        return Ok(tokenResult);
    }

    [HttpPost(AuthEndpoints.Auth.REFRESH_TOKEN)]
    public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto requestRefreshTokenDto)
    {
        TokenResponseDto? result = await authUseCase.RefreshTokenAsync(requestRefreshTokenDto);

        if (result?.AccessToken == null || result?.RefreshToken == null)
        {
            return Unauthorized("Refresh token is invalid");
        }

        return Ok(result);
    }

    [Authorize]
    [HttpGet("Authenticate")]
    public IActionResult AuthenticatedOnlyEndpoint()
    {
        // Examples
        return Ok("You are authenticated");
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin-only")]
    public IActionResult AuthenticatedAdmin()
    {
        // Examples
        return Ok("You are authenticated as an admin");
    }
}