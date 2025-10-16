using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inventoryTracking.Migrations
{
    /// <inheritdoc />
    public partial class logs1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 7, 6, 49, 4, 279, DateTimeKind.Utc).AddTicks(3609), "$2a$11$IoJMGTKUblLc7K/NAuefNuVX/dEFP60BIVwJ1UPHfYCnfgf6/vKn." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 7, 6, 47, 52, 301, DateTimeKind.Utc).AddTicks(3968), "$2a$11$NCN20EeqQl9IkTkEqERJ9.ltnKs86.PEgBPgs1dPaaMH46GQqVcGK" });
        }
    }
}
