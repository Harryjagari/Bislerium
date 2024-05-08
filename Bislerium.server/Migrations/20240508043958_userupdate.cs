using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bislerium.server.Migrations
{
    /// <inheritdoc />
    public partial class userupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8c54e0e8-40bd-4850-bc2b-c622f719aea5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cde9ace3-2f26-413a-8ee6-2a07974b16ca");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d1b9f0b6-0ee9-4255-81e5-b9a859711b7d");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "240a77f6-1e9c-4fd0-813a-ee2b59dcc7f3", "2", "Blogger", "Blogger" },
                    { "26b1a10b-aff3-4c8e-bcbf-796d85126046", "1", "Admin", "Admin" },
                    { "ca755f37-c31d-4ddd-8fdd-d1dd4929491c", "3", "Surfer", "Surfer" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "240a77f6-1e9c-4fd0-813a-ee2b59dcc7f3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "26b1a10b-aff3-4c8e-bcbf-796d85126046");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ca755f37-c31d-4ddd-8fdd-d1dd4929491c");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "8c54e0e8-40bd-4850-bc2b-c622f719aea5", "2", "Blogger", "Blogger" },
                    { "cde9ace3-2f26-413a-8ee6-2a07974b16ca", "1", "Admin", "Admin" },
                    { "d1b9f0b6-0ee9-4255-81e5-b9a859711b7d", "3", "Surfer", "Surfer" }
                });
        }
    }
}
