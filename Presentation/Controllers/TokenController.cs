using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Presentation.Controllers;

[Route("api/token")]
[ApiController]
public class TokenController:ControllerBase
{
    private readonly IServiceManager _serviceManager;
    public TokenController(IServiceManager serviceManager) => _serviceManager = serviceManager;

    /// <summary>
    /// Refreshes an authentication token using the provided refresh token.
    /// </summary>
    /// <param name="tokenDto">Contains the access and refresh tokens used to obtain a new token pair.</param>
    /// <returns>ActionResult with the refreshed TokenDto on success or an error response.</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Refresh([FromBody]TokenDto tokenDto) 
    { 
        var tokenDtoToReturn = await _serviceManager.AuthenticationService.RefreshToken(tokenDto); 
        return Ok(tokenDtoToReturn); 
    }

    /// <summary>
    /// Logs out the current session by revoking the stored refresh token.
    /// </summary>
    /// <param name="tokenDto">Contains the access and refresh tokens to identify the session.</param>
    /// <returns>204 No Content when logout succeeds.</returns>
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout([FromBody] TokenDto tokenDto)
    {
        await _serviceManager.AuthenticationService.RevokeRefreshToken(tokenDto);
        return NoContent();
    }
}