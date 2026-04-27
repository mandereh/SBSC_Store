using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    
    namespace Entities.Models;
    
    public class CartItem
    {
        [Column("CartItemId")]
        public Guid Id { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Foreign Keys
        [ForeignKey(nameof(Cart))]
        public Guid CartId { get; set; }  // CHANGED: Now references Cart, not User
        public Cart? Cart { get; set; }
        
        [ForeignKey(nameof(Product))]
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
    }