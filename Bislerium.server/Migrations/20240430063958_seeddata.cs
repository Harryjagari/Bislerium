using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bislerium.server.Migrations
{
    /// <inheritdoc />
    public partial class seeddata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "39267dd9-3753-44a4-a482-a68ced637c4a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5ab7dc93-ee19-48eb-80ef-a00f01e98846");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ceacd902-e40e-4bfc-96c9-bd7fe34ffa41");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "115052e0-bd69-46cf-9850-35b6659673a3", "1", "Admin", "Admin" },
                    { "5015c0ac-318c-40f9-bd5e-57119b085239", "2", "Blogger", "Blogger" },
                    { "b06efed2-d263-46da-856b-20cc5d21c17b", "3", "Surfer", "Surfer" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "115052e0-bd69-46cf-9850-35b6659673a3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5015c0ac-318c-40f9-bd5e-57119b085239");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b06efed2-d263-46da-856b-20cc5d21c17b");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "39267dd9-3753-44a4-a482-a68ced637c4a", "3", "Surfer", "Surfer" },
                    { "5ab7dc93-ee19-48eb-80ef-a00f01e98846", "2", "User", "User" },
                    { "ceacd902-e40e-4bfc-96c9-bd7fe34ffa41", "1", "Admin", "Admin" }
                });
        }
    }
}
