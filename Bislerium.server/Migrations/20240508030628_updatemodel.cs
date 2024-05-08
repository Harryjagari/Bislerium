using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bislerium.server.Migrations
{
    /// <inheritdoc />
    public partial class updatemodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Reactions",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ResetPasswordOTP",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetPasswordOTPIssueTime",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "12dfe6e9-d526-4de2-82d5-f420f5354bff", "3", "Surfer", "Surfer" },
                    { "313044d6-6d37-48b8-948f-3ad108d76550", "1", "Admin", "Admin" },
                    { "c6ff03c9-9e40-4b1a-bf97-6d6c2e524190", "2", "Blogger", "Blogger" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "12dfe6e9-d526-4de2-82d5-f420f5354bff");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "313044d6-6d37-48b8-948f-3ad108d76550");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c6ff03c9-9e40-4b1a-bf97-6d6c2e524190");

            migrationBuilder.DropColumn(
                name: "ResetPasswordOTP",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ResetPasswordOTPIssueTime",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Reactions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

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
    }
}
