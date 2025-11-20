using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inventoryTracking.Migrations
{
    /// <inheritdoc />
    public partial class InıtialCreateNewdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 20, 13, 19, 46, 814, DateTimeKind.Utc).AddTicks(2368), "$2a$11$lXBwaDDz/mHApgH8sekbtuhH7t40WA818CZKF8cQl9sX6IAgg9e2K" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 7, 6, 49, 4, 279, DateTimeKind.Utc).AddTicks(3609), "$2a$11$IoJMGTKUblLc7K/NAuefNuVX/dEFP60BIVwJ1UPHfYCnfgf6/vKn." });
        }
    }
}
