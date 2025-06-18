using App.Auth;
using App.Dtos.Authentication.Request;
using App.Dtos.Authentication.Response;
using App.Interfaces.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RM.Domain.Entities;
using RM.Presentation.Routes;
using RM.Presentation.Utility;

namespace RM.Presentation.Controllers.Authentication;

[ApiController]
public class AuthController(IAuthenticateUserUseCase authUseCase) : ControllerBase
{
    [HttpPost(AuthEndpoints.Auth.REGISTER)]
    public async Task<ActionResult<UserResponseDto>> Register([FromBody] UserRegisterRequestDto userRegisterRequestDto, CancellationToken cancellationToken)
    {
        UserResponseDto? userResponseDto = await authUseCase.RegisterAsync(userRegisterRequestDto, cancellationToken);

        if (userResponseDto == null)
        {
            return BadRequest("User already exists");
        }

        return Ok(userResponseDto);
        // Must return the endpoint of the profile (if it exists) and the userId
        return Created($"{AuthEndpoints.Auth.REGISTER}/{userResponseDto.UserId}", userResponseDto);
    }

    [HttpPost(AuthEndpoints.Auth.LOGIN)]
    public async Task<ActionResult<TokenResponseDto>> Login([FromBody] UserLoginRequestDto userLoginRequestDto, CancellationToken cancellationToken)
    {
        TokenResponseDto? tokenResult = await authUseCase.LoginAsync(userLoginRequestDto, cancellationToken);

        if (tokenResult == null)
        {
            return BadRequest("User not found or the password is wrong");
        }

        return Ok(tokenResult);
    }
    
    [HttpPost(AuthEndpoints.Auth.REFRESH_TOKEN)]
    public async Task<ActionResult<TokenResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto refreshTokenRequestDto, CancellationToken cancellationToken)
    {
        var tokenResponseDto = await authUseCase.RefreshTokenAsync(refreshTokenRequestDto, cancellationToken);
        
        if (tokenResponseDto == null)
        {
            return Unauthorized("Refresh token is invalid");
        }
        
        return Ok(tokenResponseDto);;
    }

    [Authorize]
    [HttpGet(AuthEndpoints.AUTHORIZED)]
    public IActionResult AuthorizeOnlyEndpoint()
    {
        // Examples
        return Ok("You are authenticated");
    }
    
    [Authorize(AuthConstants.TRUSTED_ROLE)]
    [HttpGet(AuthEndpoints.TRUSTED_USER)]
    public IActionResult TrustedOnlyEndpoint()
    {
        // Examples
        return Ok("You are a trusted user");
    }

    [Authorize(AuthConstants.ADMIN_ROLE)]
    [HttpGet(AuthEndpoints.ADMIN)]
    public IActionResult AuthenticatedAdmin()
    {
        // Examples
        return Ok("You are authenticated as an admin");
    }
}