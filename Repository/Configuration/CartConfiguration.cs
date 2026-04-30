using Entities.Enums;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configuration;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.HasKey(c => c.cartId);
        builder.Property(c => c.cartId).ValueGeneratedOnAdd();

        builder.Property(c => c.Status)
            .IsRequired();
        
        builder.Property(c => c.CreatedAt)
            .HasDefaultValueSql("NOW()");
        
        builder.Property(c => c.UpdatedAt)
            .HasDefaultValueSql("NOW()");
        
        // Unique constraint: one active cart per user
        builder.HasIndex(c => new { c.UserId, c.Status })
            .IsUnique()
            .HasFilter("[Status] = 0"); // 0 = Active
        
        // Foreign key to User
        builder.HasOne(c => c.User)
            .WithMany(u => u.Carts)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade); // If user deleted, delete their carts
        
        // Foreign key collection to CartItems
        builder.HasMany(c => c.CartItems)
            .WithOne(ci => ci.Cart)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade); // If cart deleted, delete items
    }
}