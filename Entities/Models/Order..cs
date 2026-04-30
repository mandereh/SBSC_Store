using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Enums;

namespace Entities.Models;

public class Order
{
    [Column("OrderId")]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string OrderNumber { get; set; } = default!;

    [Required]
    public string UserId { get; set; } = default!;
    public User? User { get; set; }

    [Required]
    public decimal TotalAmount { get; set; }

    [Required]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    [MaxLength(50)]
    public string? PaymentReference { get; set; }

    [MaxLength(50)]
    public string? PaymentProvider { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}