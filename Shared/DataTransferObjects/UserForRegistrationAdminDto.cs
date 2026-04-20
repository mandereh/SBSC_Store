using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects;

public record UserForRegistrationAdminDto
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? Email { get; init; }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
    public string? Password { get; init; }

    public string? Role { get; init; }
}