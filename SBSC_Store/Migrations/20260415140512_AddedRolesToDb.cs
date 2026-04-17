using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SBSC_Store.Migrations
{
    /// <inheritdoc />
    public partial class AddedRolesToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1e4d4c2b-cc61-44ea-90ac-e6c86dd5d657", null, "Customer", "CUSTOMER" },
                    { "a7fe41d0-dadb-48c9-89d7-9d8c74a6c6c7", null, "Admin", "ADMIN" }
                });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: new Guid("3d490a70-94ce-4d15-9494-5248280c2ce3"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 15, 14, 5, 11, 422, DateTimeKind.Utc).AddTicks(4133), new DateTime(2026, 4, 15, 14, 5, 11, 422, DateTimeKind.Utc).AddTicks(4134) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: new Guid("c9d4c053-49b6-410c-bc78-2d54a9991870"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 15, 14, 5, 11, 422, DateTimeKind.Utc).AddTicks(4125), new DateTime(2026, 4, 15, 14, 5, 11, 422, DateTimeKind.Utc).AddTicks(4126) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("021ca3c1-0deb-4afd-ae94-2159a8479811"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 15, 14, 5, 11, 422, DateTimeKind.Utc).AddTicks(4626), new DateTime(2026, 4, 15, 14, 5, 11, 422, DateTimeKind.Utc).AddTicks(4627) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("80abbca8-664d-4b20-b5de-024705497d4a"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 15, 14, 5, 11, 422, DateTimeKind.Utc).AddTicks(4607), new DateTime(2026, 4, 15, 14, 5, 11, 422, DateTimeKind.Utc).AddTicks(4608) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("86dba8c0-d178-41e7-938c-ed49778fb52a"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 15, 14, 5, 11, 422, DateTimeKind.Utc).AddTicks(4618), new DateTime(2026, 4, 15, 14, 5, 11, 422, DateTimeKind.Utc).AddTicks(4619) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1e4d4c2b-cc61-44ea-90ac-e6c86dd5d657");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a7fe41d0-dadb-48c9-89d7-9d8c74a6c6c7");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: new Guid("3d490a70-94ce-4d15-9494-5248280c2ce3"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 15, 13, 28, 7, 716, DateTimeKind.Utc).AddTicks(3268), new DateTime(2026, 4, 15, 13, 28, 7, 716, DateTimeKind.Utc).AddTicks(3268) });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: new Guid("c9d4c053-49b6-410c-bc78-2d54a9991870"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 15, 13, 28, 7, 716, DateTimeKind.Utc).AddTicks(3265), new DateTime(2026, 4, 15, 13, 28, 7, 716, DateTimeKind.Utc).AddTicks(3265) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("021ca3c1-0deb-4afd-ae94-2159a8479811"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 15, 13, 28, 7, 716, DateTimeKind.Utc).AddTicks(3487), new DateTime(2026, 4, 15, 13, 28, 7, 716, DateTimeKind.Utc).AddTicks(3487) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("80abbca8-664d-4b20-b5de-024705497d4a"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 15, 13, 28, 7, 716, DateTimeKind.Utc).AddTicks(3479), new DateTime(2026, 4, 15, 13, 28, 7, 716, DateTimeKind.Utc).AddTicks(3480) });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: new Guid("86dba8c0-d178-41e7-938c-ed49778fb52a"),
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 15, 13, 28, 7, 716, DateTimeKind.Utc).AddTicks(3484), new DateTime(2026, 4, 15, 13, 28, 7, 716, DateTimeKind.Utc).AddTicks(3484) });
        }
    }
}
