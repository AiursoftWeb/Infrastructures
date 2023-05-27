using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Aiursoft.Directory.Migrations
{
    public partial class AnotherWayToMarkCodeUseTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUsed",
                table: "OAuthPack");

            migrationBuilder.AddColumn<DateTime>(
                name: "UseTime",
                table: "OAuthPack",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseTime",
                table: "OAuthPack");

            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                table: "OAuthPack",
                nullable: false,
                defaultValue: false);
        }
    }
}
