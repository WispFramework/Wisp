using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wisp.Demo.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "Id", "AuthorId", "Content", "PublishDate", "Slug", "Title" },
                values: new object[] { 1, 1, "Lorem ipsum dolor", new DateTime(2025, 12, 12, 12, 12, 12, 0, DateTimeKind.Utc), "wisp-demo", "Wisp Demo" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
