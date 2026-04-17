using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Presentation.Controllers;
[Route("api/authentication")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IServiceManager _service; 
 
    public AuthenticationController(IServiceManager service) => _service = service;

    /// <summary>
    /// Registers a new user using the provided registration DTO, including the user role.
    /// </summary>
    /// <param name="userForRegistrationDto">User details for registration (email, password, etc.), with role required as either \`Admin\` or \`Customer\`.</param>
    /// <returns>201 Created on success; 400 Bad Request with model errors on failure.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto  userForRegistrationDto)
    {
        
        var result = await _service.AuthenticationService.RegisterUser(userForRegistrationDto);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.TryAddModelError(error.Code, error.Description);
            }
            return BadRequest(ModelState);
        }
        return StatusCode(201);
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token when credentials are valid.
    /// </summary>
    /// <param name="userForAuthenticationDto">Credentials for authentication (username/email and password).</param>
    /// <returns>200 OK with token DTO on success; 401 Unauthorized on failure.</returns>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto userForAuthenticationDto)
    {
        if (!await _service.AuthenticationService.ValidateUser(userForAuthenticationDto)) 
            return Unauthorized(); 
        var tokenDto = await _service.AuthenticationService 
            .CreateToken(populateExp: true); 
 
        return Ok(tokenDto);
    }
}