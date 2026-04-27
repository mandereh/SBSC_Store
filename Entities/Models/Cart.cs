using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Enums;

namespace Entities.Models;

public class Cart
{
    [Column("CartId")]
    public Guid cartId { get; set; }
    
    [Required]
    public string UserId { get; set; } // FK to IdentityUser
    public User? User { get; set; }
    
    [Required]
    public CartStatus? Status { get; set; } = CartStatus.Active;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Collection of items in this cart
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}