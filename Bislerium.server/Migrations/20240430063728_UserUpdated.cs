using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bislerium.server.Migrations
{
    /// <inheritdoc />
    public partial class UserUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1f1b8e90-aa46-4abb-9a7a-fc5014325722");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c6bb87f5-530d-40ef-835f-e6efefff53d2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f929dc26-6759-4538-a51d-c820188da604");

            migrationBuilder.AlterColumn<string>(
                name: "ProfilePictureUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "ProfilePictureUrl",
                table: "AspNetUsers",
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
                    { "1f1b8e90-aa46-4abb-9a7a-fc5014325722", "2", "User", "User" },
                    { "c6bb87f5-530d-40ef-835f-e6efefff53d2", "3", "Surfer", "Surfer" },
                    { "f929dc26-6759-4538-a51d-c820188da604", "1", "Admin", "Admin" }
                });
        }
    }
}
