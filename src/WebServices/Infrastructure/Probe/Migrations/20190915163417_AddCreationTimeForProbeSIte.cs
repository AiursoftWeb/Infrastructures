using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Aiursoft.Probe.Migrations
{
    public partial class AddCreationTimeForProbeSIte : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "Sites",
                nullable: false,
                defaultValue: new DateTime(2019, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "Sites");
        }
    }
}
