using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inventoryTracking.Migrations
{
    /// <inheritdoc />
    public partial class test2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 19, 8, 31, 20, 676, DateTimeKind.Utc).AddTicks(504), "$2a$11$Oo7o/xNsQC7ivxjiwGefWeIvPWgawfTBckS96S.AFQp98FuZpqRZS" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 19, 8, 24, 29, 370, DateTimeKind.Utc).AddTicks(6536), "$2a$11$EToYt7fOnt/S7v8f1FouHeNSTfs73Z7mJnIiRAyEzQ6RJfrZsdx56" });
        }
    }
}
