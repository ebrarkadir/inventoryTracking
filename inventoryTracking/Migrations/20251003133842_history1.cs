using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inventoryTracking.Migrations
{
    /// <inheritdoc />
    public partial class history1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 3, 13, 38, 42, 24, DateTimeKind.Utc).AddTicks(7843), "$2a$11$Wri9C9TQBxNHa.UtXYdPL.ZxFAiL7xpLez9h2WS41gFjwavXaAWTC" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 3, 11, 43, 16, 824, DateTimeKind.Utc).AddTicks(1006), "$2a$11$THmsgm.WKiURoWs2aSWoOu0OqAljGl9oMHvsWClW7UYGZyyWaagRO" });
        }
    }
}
