using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

internal sealed class AuthenticationService : IAuthenticationService
{
    private const string CustomerRole = "Customer";
    private readonly ILoggerManager _logger; 
    private readonly IMapper _mapper; 
    private readonly UserManager<User> _userManager; 
    private readonly IConfiguration _configuration;
    private User? _user;
    // private readonly RoleManager<TRole>  _roleManager;
 
    public AuthenticationService(ILoggerManager logger, IMapper mapper,  
        UserManager<User> userManager, IConfiguration configuration) 
    { 
        _logger = logger; 
        _mapper = mapper; 
        _userManager = userManager; 
        _configuration = configuration; 
    }


    public async Task<(IdentityResult identityResult, RegistrationResponseDto? registrationResponseDto)>
        AdminRegisterUser(UserForRegistrationAdminDto userForRegistrationAdminDto)
    {
        var user = _mapper.Map<User>(userForRegistrationAdminDto);
    
        var email = userForRegistrationAdminDto.Email?.Trim();
        user.Email = email;
        user.UserName = email;
    
        var result = await _userManager.CreateAsync(user, userForRegistrationAdminDto.Password);
        if (!result.Succeeded)
            return (result, null);
    
        var role = string.IsNullOrWhiteSpace(userForRegistrationAdminDto.Role)
            ? CustomerRole
            : userForRegistrationAdminDto.Role.Trim();
    
        var allowedRoles = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Admin", "Customer" };
        if (!allowedRoles.Contains(role))
        {
            return (IdentityResult.Failed(new IdentityError
            {
                Code = "InvalidRole",
                Description = $"Unsupported role: {role}"
            }), null);
        }
    
        var roleResult = await _userManager.AddToRoleAsync(user, role);
        if (!roleResult.Succeeded)
            return (roleResult, null);
    
        _user = user;
        var tokenDto = await CreateToken(populateExp: true);
    
        var registeredUser = new RegisteredUserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = role
        };
    
        return (IdentityResult.Success, new RegistrationResponseDto(registeredUser, tokenDto));
    }
    
    public async Task<(IdentityResult Result, RegistrationResponseDto? registrationResponseDto)> RegisterUser(
        UserForRegistrationDto userForRegistration) 
    { 
        var user = _mapper.Map<User>(userForRegistration);
        var email = userForRegistration.Email?.Trim();
        user.Email = email;
        user.UserName = email;
        

        var result = await _userManager.CreateAsync(user, userForRegistration.Password); 
        if (!result.Succeeded)
            return (result, null);

        // Public registration always assigns Customer implicitly.
        var roleResult = await _userManager.AddToRoleAsync(user, CustomerRole);
        if (!roleResult.Succeeded)
            return (roleResult, null);

        _user = user;
        var tokenDto = await CreateToken(populateExp: true);

        var registeredUser = new RegisteredUserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = CustomerRole
        };

        var response = new RegistrationResponseDto(registeredUser, tokenDto);
        return (IdentityResult.Success, response);
    } 
    public async Task<bool> ValidateUser(UserForAuthenticationDto userForAuth) 
    { 
        _user = await _userManager.FindByEmailAsync(userForAuth.Email); 
 
        var result = (_user != null && await _userManager.CheckPasswordAsync(_user, 
            userForAuth.Password)); 
        if (!result) 
            _logger.LogWarn($"{nameof(ValidateUser)}: Authentication failed. Wrong email or password."); 
 
        return result; 
    } 
 
    public async Task<TokenDto> CreateToken(bool populateExp) 
    { 
        var signingCredentials = GetSigningCredentials(); 
        var claims = await GetClaims(); 
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims); 
 
        var refreshToken = GenerateRefreshToken(); 
 
        _user.RefreshToken = refreshToken; 
 
        if(populateExp) 
            _user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); 
 
        await _userManager.UpdateAsync(_user); 
 
        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions); 
 
        return new TokenDto(accessToken, refreshToken); 
    } 
 
    private SigningCredentials GetSigningCredentials() 
    { 
        var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET")); 
        var secret = new SymmetricSecurityKey(key); 
 
        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256); 
    } 
 
    private async Task<List<Claim>> GetClaims() 
    { 
        var claims = new List<Claim> 
        { 
            new Claim(ClaimTypes.Email, _user.Email),
            // Keep Name aligned with email so Identity.Name is available during refresh.
            new Claim(ClaimTypes.Name, _user.UserName ?? _user.Email)
        }; 
 
        var roles = await _userManager.GetRolesAsync(_user); 
        foreach (var role in roles) 
        { 
            claims.Add(new Claim(ClaimTypes.Role, role)); 
        } 
 
        return claims; 
    }
    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, 
        List<Claim> claims) 
    { 
        var jwtSettings = _configuration.GetSection("JwtSettings"); 
        var tokenOptions = new JwtSecurityToken 
        ( 
            issuer: jwtSettings["validIssuer"], 
            audience: jwtSettings["validAudience"], 
            claims: claims, 
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["expires"])), 
            signingCredentials: signingCredentials 
        ); 
        return tokenOptions; 
    } 
    
    private string GenerateRefreshToken() 
    { 
        var randomNumber = new byte[32]; 
        using (var rng = RandomNumberGenerator.Create()) 
        { 
            rng.GetBytes(randomNumber); 
            return Convert.ToBase64String(randomNumber);
 
        } 
    } 
    
    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token) 
    { 
        var jwtSettings = _configuration.GetSection("JwtSettings"); 
 
        var tokenValidationParameters = new TokenValidationParameters 
        { 
            ValidateAudience = true, 
            ValidateIssuer = true, 
            ValidateIssuerSigningKey = true, 
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET"))), 
            ValidateLifetime = true, 
            ValidIssuer = jwtSettings["validIssuer"], 
            ValidAudience = jwtSettings["validAudience"] 
        }; 
 
        var tokenHandler = new JwtSecurityTokenHandler(); 
        SecurityToken securityToken; 
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out 
            securityToken); 
 
        var jwtSecurityToken = securityToken as JwtSecurityToken; 
        if (jwtSecurityToken == null || 
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, 
                StringComparison.InvariantCultureIgnoreCase)) 
        { 
            throw new SecurityTokenException("Invalid token"); 
        } 
 
        return principal; 
    }
    private async Task<User?> GetUserFromPrincipalAsync(ClaimsPrincipal principal)
    {
        var email = principal.FindFirstValue(ClaimTypes.Email);
        var userName = principal.Identity?.Name;

        User? user = null;
        if (!string.IsNullOrWhiteSpace(userName))
            user = await _userManager.FindByNameAsync(userName);

        if (user is null && !string.IsNullOrWhiteSpace(email))
            user = await _userManager.FindByEmailAsync(email);

        return user;
    }

    public async Task<TokenDto> RefreshToken(TokenDto tokenDto) 
    {
        if (string.IsNullOrWhiteSpace(tokenDto.AccessToken) || string.IsNullOrWhiteSpace(tokenDto.RefreshToken))
            throw new RefreshTokenBadRequest();

        var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);
        var user = await GetUserFromPrincipalAsync(principal);

        if (user == null || user.RefreshToken != tokenDto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            throw new RefreshTokenBadRequest();

        _user = user;
        return await CreateToken(populateExp: false);
    }

    public async Task RevokeRefreshToken(TokenDto tokenDto)
    {
        if (string.IsNullOrWhiteSpace(tokenDto.AccessToken) || string.IsNullOrWhiteSpace(tokenDto.RefreshToken))
            throw new RefreshTokenBadRequest();

        var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);
        var user = await GetUserFromPrincipalAsync(principal);

        if (user == null || user.RefreshToken != tokenDto.RefreshToken)
            throw new RefreshTokenBadRequest();

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
    }
}