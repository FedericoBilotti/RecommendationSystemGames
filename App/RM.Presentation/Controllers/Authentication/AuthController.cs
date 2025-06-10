using App.Dtos.Authentication.Request;
using App.Dtos.Authentication.Response;
using App.Interfaces.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RM.Domain.Entities;
using RM.Presentation.Auth;
using RM.Presentation.Routes;

namespace RM.Presentation.Controllers.Authentication;

[ApiController]
public class AuthController(IAuthenticateUserUseCase authUseCase) : ControllerBase
{
    [HttpPost(AuthEndpoints.Auth.REGISTER)]
    public async Task<ActionResult<User>> Register([FromBody] UserRegisterRequestDto userRegisterRequestDto, CancellationToken cancellationToken)
    {
        UserResponseDto? userResponseDto = await authUseCase.RegisterAsync(userRegisterRequestDto, cancellationToken);

        if (userResponseDto == null)
        {
            return BadRequest("User already exists");
        }

        return Created($"{AuthEndpoints.Auth.REGISTER}/{userResponseDto.UserId}", userResponseDto);
    }

    [HttpPost(AuthEndpoints.Auth.LOGIN)]
    public async Task<ActionResult<string>> Login(UserLoginRequestDto userLoginRequestDto, CancellationToken cancellationToken)
    {
        TokenResponseDto? tokenResult = await authUseCase.LoginAsync(userLoginRequestDto, cancellationToken);

        if (tokenResult == null)
        {
            return BadRequest("User not found or the password is wrong");
        }

        return Ok(tokenResult);
    }

    [HttpPost(AuthEndpoints.Auth.REFRESH_TOKEN)]
    public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto requestRefreshTokenDto, CancellationToken cancellationToken)
    {
        TokenResponseDto? result = await authUseCase.RefreshTokenAsync(requestRefreshTokenDto, cancellationToken);

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