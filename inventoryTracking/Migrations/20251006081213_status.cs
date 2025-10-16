using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inventoryTracking.Migrations
{
    /// <inheritdoc />
    public partial class status : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastActionDate",
                table: "Inventories",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Inventories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 6, 8, 12, 12, 840, DateTimeKind.Utc).AddTicks(9253), "$2a$11$7wRz2eur7F72Nh9dKsTvI.0rmwWW35wFrCeD/9uxN3dKkhlfKSpi." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastActionDate",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Inventories");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 3, 14, 16, 34, 664, DateTimeKind.Utc).AddTicks(5620), "$2a$11$izTJBJbDl6s8499oT9Sg0u3ou2NmLOCIAfXQ.HJ339J8em.umYq3C" });
        }
    }
}
