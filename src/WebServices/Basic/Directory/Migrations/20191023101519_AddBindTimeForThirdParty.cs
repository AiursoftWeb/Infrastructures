using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Aiursoft.Directory.Migrations
{
    public partial class AddBindTimeForThirdParty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BindTime",
                table: "ThirdPartyAccounts",
                nullable: false,
                defaultValue: new DateTime(2019, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BindTime",
                table: "ThirdPartyAccounts");
        }
    }
}
