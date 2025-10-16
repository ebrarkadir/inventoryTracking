using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inventoryTracking.Migrations
{
    /// <inheritdoc />
    public partial class isActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Inventories",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 6, 9, 6, 5, 936, DateTimeKind.Utc).AddTicks(7663), "$2a$11$fkud9NR/F7rBjE4eFTKcmOpXiofz9.3niQf1KZO6P1X4e.ylhDDu." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Inventories");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 6, 8, 12, 12, 840, DateTimeKind.Utc).AddTicks(9253), "$2a$11$7wRz2eur7F72Nh9dKsTvI.0rmwWW35wFrCeD/9uxN3dKkhlfKSpi." });
        }
    }
}
