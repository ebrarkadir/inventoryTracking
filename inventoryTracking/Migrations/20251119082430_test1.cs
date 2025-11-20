using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inventoryTracking.Migrations
{
    /// <inheritdoc />
    public partial class test1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 19, 8, 24, 29, 370, DateTimeKind.Utc).AddTicks(6536), "$2a$11$EToYt7fOnt/S7v8f1FouHeNSTfs73Z7mJnIiRAyEzQ6RJfrZsdx56" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 11, 17, 13, 52, 17, 782, DateTimeKind.Utc).AddTicks(464), "$2a$11$qC9J2FCzDUrCYDglBwun0eIa/dCnp2D0B5YMcZe6Sv.Mm7GQZsoou" });
        }
    }
}
