using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OleMissChatbox.Migrations
{
    public partial class SeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "CreatedDate", "Email", "FirstName", "LastName", "Password" },
                values: new object[] { 1, new DateTime(2022, 3, 27, 21, 41, 59, 738, DateTimeKind.Local).AddTicks(1911), "btcorrie@gmail.com", "Ben", "Corrie", "123" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1);
        }
    }
}
