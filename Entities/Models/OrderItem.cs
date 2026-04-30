using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

public class OrderItem
{
    [Column("OrderItemId")]
    public Guid Id { get; set; }


    public Guid ProductId { get; set; }

    [MaxLength(255)]
    public string? ProductName { get; set; }

    public decimal ProductPrice { get; set; }

    public int Quantity { get; set; }

    public decimal TotalPrice { get; set; }
    [ForeignKey(nameof(Order))]
    public Guid OrderId { get; set; }
    public Order? Order { get; set; }

}