using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configuration;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{

    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasData(
            new Product
            {
                Id = new Guid("80abbca8-664d-4b20-b5de-024705497d4a"),
                Name = "The Intellectual Life",
                Description = "The Intellectual Life of the catholic scholar",
                Price = 1000,
                ImageUrl = "",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CategoryId = new Guid("c9d4c053-49b6-410c-bc78-2d54a9991870")
            },
            new Product
            {
                Id = new Guid("86dba8c0-d178-41e7-938c-ed49778fb52a"),
                Name = "Fair Game",
                Description = "Fair Game 1992",
                Price = 2000,
                ImageUrl = "",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CategoryId = new Guid("3d490a70-94ce-4d15-9494-5248280c2ce3")
            },
            new Product
            {
                Id = new Guid("021ca3c1-0deb-4afd-ae94-2159a8479811"),
                Name = "Les Sources",
                Description = "Les Sources by Pere Gratry",
                Price = 1000,
                ImageUrl = "",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CategoryId = new Guid("c9d4c053-49b6-410c-bc78-2d54a9991870")
            }
            );
    }
    
}