using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wisp.Demo.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Password", "Role", "Username" },
                values: new object[] { 1, "$2a$11$5btcGkcxRRkAc1xoqY.5.u./yDYLcXO5H9tU8dHdKcH2zPmLwq1rm", "admin", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
