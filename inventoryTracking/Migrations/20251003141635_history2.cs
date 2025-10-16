using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inventoryTracking.Migrations
{
    /// <inheritdoc />
    public partial class history2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewValues",
                table: "InventoryHistories");

            migrationBuilder.DropColumn(
                name: "OldValues",
                table: "InventoryHistories");

            migrationBuilder.AlterColumn<string>(
                name: "ChangedBy",
                table: "InventoryHistories",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Snapshot",
                table: "InventoryHistories",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 3, 14, 16, 34, 664, DateTimeKind.Utc).AddTicks(5620), "$2a$11$izTJBJbDl6s8499oT9Sg0u3ou2NmLOCIAfXQ.HJ339J8em.umYq3C" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Snapshot",
                table: "InventoryHistories");

            migrationBuilder.AlterColumn<string>(
                name: "ChangedBy",
                table: "InventoryHistories",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "NewValues",
                table: "InventoryHistories",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "OldValues",
                table: "InventoryHistories",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 3, 13, 38, 42, 24, DateTimeKind.Utc).AddTicks(7843), "$2a$11$Wri9C9TQBxNHa.UtXYdPL.ZxFAiL7xpLez9h2WS41gFjwavXaAWTC" });
        }
    }
}
