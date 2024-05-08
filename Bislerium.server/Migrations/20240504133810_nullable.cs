using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bislerium.server.Migrations
{
    /// <inheritdoc />
    public partial class nullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0541c29a-b33b-4274-aef3-bd972a67ed2e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "38e0c452-df5a-43e2-959a-70f96dcaea8b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "40f16ce0-2032-4fa7-8cf7-3d16f502b223");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModifiedDate",
                table: "Comments",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0b38fc5e-3fc4-4591-8e03-a4e14f39ff29", "1", "Admin", "Admin" },
                    { "3812ddc4-54d3-4f06-b8f7-7c7e15995a90", "2", "Blogger", "Blogger" },
                    { "886ce13f-b7f5-4011-b72b-b49b2ef2bed6", "3", "Surfer", "Surfer" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0b38fc5e-3fc4-4591-8e03-a4e14f39ff29");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3812ddc4-54d3-4f06-b8f7-7c7e15995a90");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "886ce13f-b7f5-4011-b72b-b49b2ef2bed6");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastModifiedDate",
                table: "Comments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0541c29a-b33b-4274-aef3-bd972a67ed2e", "2", "Blogger", "Blogger" },
                    { "38e0c452-df5a-43e2-959a-70f96dcaea8b", "1", "Admin", "Admin" },
                    { "40f16ce0-2032-4fa7-8cf7-3d16f502b223", "3", "Surfer", "Surfer" }
                });
        }
    }
}
