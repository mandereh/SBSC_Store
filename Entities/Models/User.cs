using Microsoft.AspNetCore.Identity;
namespace Entities.Models;

public class User : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    // Add this collection - User can have multiple carts
    public ICollection<Cart>? Carts { get; set; }
}