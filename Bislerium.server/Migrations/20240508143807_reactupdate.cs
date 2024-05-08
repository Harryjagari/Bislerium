using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bislerium.server.Migrations
{
    /// <inheritdoc />
    public partial class reactupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a5e13b73-55ca-439f-9f9d-a266bcd6e864");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c1983897-2e19-44d5-a4e6-e4fe902bf261");

            migrationBuilder.AlterColumn<Guid>(
                name: "BlogPostId",
                table: "Reactions",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "330eb5c4-009b-4145-bf6c-8c9fea4d1950", "2", "Blogger", "Blogger" },
                    { "d05f94b7-cd22-4466-af4e-e02b4d0f7893", "1", "Admin", "Admin" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "330eb5c4-009b-4145-bf6c-8c9fea4d1950");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d05f94b7-cd22-4466-af4e-e02b4d0f7893");

            migrationBuilder.AlterColumn<Guid>(
                name: "BlogPostId",
                table: "Reactions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "a5e13b73-55ca-439f-9f9d-a266bcd6e864", "1", "Admin", "Admin" },
                    { "c1983897-2e19-44d5-a4e6-e4fe902bf261", "2", "Blogger", "Blogger" }
                });
        }
    }
}
