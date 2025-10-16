using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inventoryTracking.Migrations
{
    /// <inheritdoc />
    public partial class seedAdmin1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 9, 26, 10, 29, 37, 360, DateTimeKind.Utc).AddTicks(1669), "$2a$11$l/wBbtuDlNDnWQXQCDNIpekCY6Ci5QBz4vuIj.J5mbWm.u5.FKOK2" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 9, 26, 9, 55, 4, 666, DateTimeKind.Utc).AddTicks(6015), "$2a$11$AsHk3PYJz.rYC8Z1xyzYCeTAF1GPc9QIZ2Oc5LaR/i6Mz1UzuEdTy" });
        }
    }
}
