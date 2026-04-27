using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configuration;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedOnAdd();
        
        builder.Property(r => r.Rating)
            .IsRequired();
        
        builder.Property(r => r.Comment)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.Property(r => r.CreatedAt)
            .HasDefaultValueSql("NOW()");
        
        // Foreign key to Product
        builder.HasOne(r => r.Product)
            .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Cascade); // If product deleted, delete reviews
        
        // Foreign key to User
        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.SetNull); // If user deleted, keep review but null userId
    }
}