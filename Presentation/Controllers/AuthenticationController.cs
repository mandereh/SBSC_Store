using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Presentation.Controllers;
[Route("api/authentication")]
[ApiController]
[Produces("application/json")]
public class AuthenticationController : ControllerBase
{
    private readonly IServiceManager _service; 
 
    public AuthenticationController(IServiceManager service) => _service = service;
    
    
    
    
    /// <summary>
    /// Allows an authenticated admin to create a new user and assign a role to that user via the admin registration endpoint (`POST api/authentication/admin/register`).
    /// </summary>
    /// <param name="userForRegistrationDto">
    /// Registration payload provided by an authenticated admin. Roles included in the payload will be assigned to the created user.
    /// </param>
    /// <returns>
    /// 201 Created with <see cref="RegistrationResponseDto"/> when successful; otherwise an error response.
    /// </returns>
    /// <response code="201">User created successfully and roles assigned.</response>
    /// <response code="400">The request is invalid or registration failed validation.</response>
    /// <response code="401">The caller is not authenticated.</response>
    /// <response code="403">The caller is authenticated but not in the Admin role.</response>
    [Authorize(Roles = "Admin")]
    [HttpPost("admin/register")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(RegistrationResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AdminRegisterUser([FromBody] UserForRegistrationAdminDto userForRegistrationDto)
    {
        var (result, response) = await _service.AuthenticationService.AdminRegisterUser(userForRegistrationDto);
    
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.TryAddModelError(error.Code, error.Description);
    
            return BadRequest(ModelState);
        }
    
        return StatusCode(StatusCodes.Status201Created, response);
    }
    
    
    

    /// <summary>
    /// Registers a new user, assigns the Customer role implicitly, and returns user details with JWT tokens.
    /// </summary>
    /// <param name="userForRegistrationDto">User details for registration (email, password, etc.). Role is assigned implicitly and is not accepted from the request payload.</param>
    /// <returns>201 Created with user details and token on success; 400 Bad Request with model errors on failure.</returns>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(RegistrationResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto  userForRegistrationDto)
    {
        var (result, response) = await _service.AuthenticationService.RegisterUser(userForRegistrationDto);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.TryAddModelError(error.Code, error.Description);
            }
            return BadRequest(ModelState);
        }

        return StatusCode(StatusCodes.Status201Created, response);
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token when credentials are valid.
    /// </summary>
    /// <param name="userForAuthenticationDto">Credentials for authentication (username/email and password).</param>
    /// <returns>200 OK with token DTO on success; 401 Unauthorized on failure.</returns>
    [HttpPost("login")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
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