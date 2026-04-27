using Entities.Models;
        using Microsoft.EntityFrameworkCore;
        using Microsoft.EntityFrameworkCore.Metadata.Builders;
        
        namespace Repository.Configuration;
        
        public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
        {
            public void Configure(EntityTypeBuilder<CartItem> builder)
            {
                builder.HasKey(c => c.Id);
                builder.Property(c => c.Id).ValueGeneratedOnAdd();
                
                builder.Property(c => c.Quantity)
                    .IsRequired();
                
                builder.Property(c => c.CreatedAt)
                    .HasDefaultValueSql("NOW()");
                
                // Unique constraint: one product per cart
                builder.HasIndex(c => new { c.CartId, c.ProductId })
                    .IsUnique();
                
                // Foreign key to Cart
                builder.HasOne(c => c.Cart)
                    .WithMany(cart => cart.CartItems)
                    .HasForeignKey(c => c.CartId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                // Foreign key to Product
                builder.HasOne(c => c.Product)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(c => c.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            }
        }