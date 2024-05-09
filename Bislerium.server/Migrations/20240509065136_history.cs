using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bislerium.server.Migrations
{
    /// <inheritdoc />
    public partial class history : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "330eb5c4-009b-4145-bf6c-8c9fea4d1950");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d05f94b7-cd22-4466-af4e-e02b4d0f7893");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedImageUrl",
                table: "BlogPostUpdateHistories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "OriginalImageUrl",
                table: "BlogPostUpdateHistories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0065062b-da2b-4c72-9748-6c303f143e98", "2", "Blogger", "Blogger" },
                    { "082da602-7230-4c78-af5f-fc55294d3efa", "1", "Admin", "Admin" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0065062b-da2b-4c72-9748-6c303f143e98");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "082da602-7230-4c78-af5f-fc55294d3efa");

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedImageUrl",
                table: "BlogPostUpdateHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OriginalImageUrl",
                table: "BlogPostUpdateHistories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "330eb5c4-009b-4145-bf6c-8c9fea4d1950", "2", "Blogger", "Blogger" },
                    { "d05f94b7-cd22-4466-af4e-e02b4d0f7893", "1", "Admin", "Admin" }
                });
        }
    }
}
