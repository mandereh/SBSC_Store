using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects;

public record UserForAuthenticationDto
{
    [Required(ErrorMessage = "Email is required"), EmailAddress] 
    public string? Email { get; init; } 
    [Required(ErrorMessage = "Password name is required")] 
    public string? Password { get; init; }
}