using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inventoryTracking.Migrations
{
    /// <inheritdoc />
    public partial class log : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "Logs",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<string>(
                name: "Details",
                table: "Logs",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Logs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 9, 30, 13, 13, 22, 533, DateTimeKind.Utc).AddTicks(958), "$2a$11$A9fo5Pm2rOTy5LHLVmDa0eSw3IkupTgAENraDq8cs7wV44oV18hzO" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Details",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Logs");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Logs",
                newName: "Timestamp");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 9, 30, 7, 36, 4, 454, DateTimeKind.Utc).AddTicks(3520), "$2a$11$sEtRNnR9tDvYL6JhhY1LQOjKG9Ny8N0ycUDyoCHrdv0u9RwzJip8i" });
        }
    }
}
