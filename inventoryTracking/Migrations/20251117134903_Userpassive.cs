using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inventoryTracking.Migrations
{
    /// <inheritdoc />
    public partial class Userpassive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "IsActive", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 17, 13, 49, 2, 764, DateTimeKind.Utc).AddTicks(1866), true, "$2a$11$5Zp8ld9ny7iu17COBPfzhenj/vOIPvdwaIu0Y.HRdrdDKKQcoJeQG" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 21, 6, 48, 20, 17, DateTimeKind.Utc).AddTicks(1991), "$2a$11$MeRtCuhg.CfxU9PY5gdYqeVUTNiRZsOw4x6HDRBAfhmkJwA1o3squ" });
        }
    }
}
