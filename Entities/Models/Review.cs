using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

public class Review
{
    [Column("ReviewId")]
    public Guid Id { get; set; }
    
    [Required]
    public int Rating { get; set; } // 1-5 stars
    
    [Required]
    public string? Comment { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Foreign Keys
    [ForeignKey(nameof(Product))]
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
    
    [ForeignKey(nameof(User))]
    public string? UserId { get; set; }
    public User? User { get; set; }
}