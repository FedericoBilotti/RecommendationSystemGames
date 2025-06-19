using App.Auth;
using App.Dtos.Authentication.Request;
using App.Dtos.Authentication.Response;
using App.Dtos.Games.Responses;
using App.Interfaces.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RM.Presentation.Routes;

namespace RM.Presentation.Controllers.Authentication;

[ApiController]
public class AuthController(IAuthenticateUserUseCase authUseCase) : ControllerBase
{
    [HttpPost(AuthEndpoints.Auth.REGISTER)]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserResponseDto>> Register([FromBody] UserRegisterRequestDto userRegisterRequestDto, CancellationToken cancellationToken)
    {
        UserResponseDto? userResponseDto = await authUseCase.RegisterAsync(userRegisterRequestDto, cancellationToken);

        if (userResponseDto == null)
        {
            return Conflict("User already exists");
        }

        return CreatedAtAction(nameof(GetUser), new { userId = userResponseDto.UserId }, userResponseDto);
    }

    [Authorize]
    [HttpGet(AuthEndpoints.Auth.GET)]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponseDto>> GetUser([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        UserResponseDto? userResponseDto = await authUseCase.GetUserAsync(userId, cancellationToken);

        if (userResponseDto == null)
        {
            return NotFound("User not found");
        }

        return Ok(userResponseDto);
    }

    [HttpPost(AuthEndpoints.Auth.LOGIN)]
    [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TokenResponseDto>> Login([FromBody] UserLoginRequestDto userLoginRequestDto, CancellationToken cancellationToken)
    {
        TokenResponseDto? tokenResult = await authUseCase.LoginAsync(userLoginRequestDto, cancellationToken);

        if (tokenResult == null)
        {
            return NotFound("User not found or the password is wrong");
        }

        return Ok(tokenResult);
    }
    
    [HttpPost(AuthEndpoints.Auth.REFRESH_TOKEN)]
    [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TokenResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto refreshTokenRequestDto, CancellationToken cancellationToken)
    {
        var tokenResponseDto = await authUseCase.RefreshTokenAsync(refreshTokenRequestDto, cancellationToken);
        
        if (tokenResponseDto == null)
        {
            return Unauthorized("Refresh token is invalid");
        }
        
        return Ok(tokenResponseDto);;
    }
    
    #region Examples

    [Authorize]
    [HttpGet(AuthEndpoints.AUTHORIZED)]
    public IActionResult AuthorizeOnlyEndpoint()
    {
        return Ok("You are authenticated");
    }
    
    [Authorize(AuthConstants.TRUSTED_ROLE)]
    [HttpGet(AuthEndpoints.TRUSTED_USER)]
    public IActionResult TrustedOnlyEndpoint()
    {
        return Ok("You are a trusted user");
    }

    [Authorize(AuthConstants.ADMIN_ROLE)]
    [HttpGet(AuthEndpoints.ADMIN)]
    public IActionResult AuthenticatedAdmin()
    {
        return Ok("You are authenticated as an admin");
    }
    
    #endregion
}