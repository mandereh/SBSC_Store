using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SBSC_Store.Migrations
{
    /// <inheritdoc />
    public partial class InitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CreatedAt", "Description", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("3d490a70-94ce-4d15-9494-5248280c2ce3"), new DateTime(2026, 4, 10, 23, 36, 8, 987, DateTimeKind.Utc).AddTicks(1413), "Music Category", "Music", new DateTime(2026, 4, 10, 23, 36, 8, 987, DateTimeKind.Utc).AddTicks(1414) },
                    { new Guid("c9d4c053-49b6-410c-bc78-2d54a9991870"), new DateTime(2026, 4, 10, 23, 36, 8, 987, DateTimeKind.Utc).AddTicks(1409), "Books Category", "Books", new DateTime(2026, 4, 10, 23, 36, 8, 987, DateTimeKind.Utc).AddTicks(1411) }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryId", "CreatedAt", "Description", "ImageUrl", "Name", "Price", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("021ca3c1-0deb-4afd-ae94-2159a8479811"), new Guid("c9d4c053-49b6-410c-bc78-2d54a9991870"), new DateTime(2026, 4, 10, 23, 36, 8, 987, DateTimeKind.Utc).AddTicks(1728), "Les Sources by Pere Gratry", "", "Les Sources", 1000m, new DateTime(2026, 4, 10, 23, 36, 8, 987, DateTimeKind.Utc).AddTicks(1729) },
                    { new Guid("80abbca8-664d-4b20-b5de-024705497d4a"), new Guid("c9d4c053-49b6-410c-bc78-2d54a9991870"), new DateTime(2026, 4, 10, 23, 36, 8, 987, DateTimeKind.Utc).AddTicks(1720), "The Intellectual Life of the catholic scholar", "", "The Intellectual Life", 1000m, new DateTime(2026, 4, 10, 23, 36, 8, 987, DateTimeKind.Utc).AddTicks(1721) },
                    { new Guid("86dba8c0-d178-41e7-938c-ed49778fb52a"), new Guid("3d490a70-94ce-4d15-9494-5248280c2ce3"), new DateTime(2026, 4, 10, 23, 36, 8, 987, DateTimeKind.Utc).AddTicks(1725), "Fair Game 1992", "", "Fair Game", 2000m, new DateTime(2026, 4, 10, 23, 36, 8, 987, DateTimeKind.Utc).AddTicks(1725) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("021ca3c1-0deb-4afd-ae94-2159a8479811"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("80abbca8-664d-4b20-b5de-024705497d4a"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("86dba8c0-d178-41e7-938c-ed49778fb52a"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: new Guid("3d490a70-94ce-4d15-9494-5248280c2ce3"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: new Guid("c9d4c053-49b6-410c-bc78-2d54a9991870"));
        }
    }
}
