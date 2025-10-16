using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inventoryTracking.Migrations
{
    /// <inheritdoc />
    public partial class adetDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Inventories");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 6, 10, 5, 25, 501, DateTimeKind.Utc).AddTicks(9811), "$2a$11$A7/kxZULLSQad6tQcaMrguN71NNQzx0FMKIbUznDfKX7GMp5Wa/ui" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Inventories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 6, 9, 6, 5, 936, DateTimeKind.Utc).AddTicks(7663), "$2a$11$fkud9NR/F7rBjE4eFTKcmOpXiofz9.3niQf1KZO6P1X4e.ylhDDu." });
        }
    }
}
