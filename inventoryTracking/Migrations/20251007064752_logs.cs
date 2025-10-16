using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inventoryTracking.Migrations
{
    /// <inheritdoc />
    public partial class logs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "EntityId",
                table: "Logs",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EntityType",
                table: "Logs",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 7, 6, 47, 52, 301, DateTimeKind.Utc).AddTicks(3968), "$2a$11$NCN20EeqQl9IkTkEqERJ9.ltnKs86.PEgBPgs1dPaaMH46GQqVcGK" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "Logs");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 6, 10, 5, 25, 501, DateTimeKind.Utc).AddTicks(9811), "$2a$11$A7/kxZULLSQad6tQcaMrguN71NNQzx0FMKIbUznDfKX7GMp5Wa/ui" });
        }
    }
}
