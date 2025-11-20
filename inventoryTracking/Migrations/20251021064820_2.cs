using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inventoryTracking.Migrations
{
    /// <inheritdoc />
    public partial class _2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 21, 6, 48, 20, 17, DateTimeKind.Utc).AddTicks(1991), "$2a$11$MeRtCuhg.CfxU9PY5gdYqeVUTNiRZsOw4x6HDRBAfhmkJwA1o3squ" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 20, 13, 19, 46, 814, DateTimeKind.Utc).AddTicks(2368), "$2a$11$lXBwaDDz/mHApgH8sekbtuhH7t40WA818CZKF8cQl9sX6IAgg9e2K" });
        }
    }
}
