using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompletedAuthenticationAuthorization.Migrations
{
    /// <inheritdoc />
    public partial class secondmig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "$2a$12$Coh2usqFXkDAI3uF4izz2.RlK8mozAjKS9SgfnSDBTUgYQF03u8M2" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 9, 11, 38, 5, 596, DateTimeKind.Utc).AddTicks(4970), "$2a$11$MuTc96rsY0TLUHuPc/.h0u1WSubSNDrvNRs.Dn.Tzjf2UlqHg6I6u" });
        }
    }
}
