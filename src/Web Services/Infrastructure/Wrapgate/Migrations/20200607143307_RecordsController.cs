using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Wrapgate.Migrations
{
    public partial class RecordsController : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "Records",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "Records");
        }
    }
}
