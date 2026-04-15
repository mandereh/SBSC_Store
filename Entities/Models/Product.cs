using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

public class Product
{
    [Column("ProductId")]
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Product Name is required")]
    public string? Name { get; set; }
    [Required(ErrorMessage = "Product Price is required")]
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    [Required(ErrorMessage = "Product Description is required")]
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    [ForeignKey(nameof(Category))]
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
}