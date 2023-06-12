using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aiursoft.Developer.Migrations
{
    public partial class CleanSomeProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppCategory",
                table: "Apps");

            migrationBuilder.DropColumn(
                name: "AppPlatform",
                table: "Apps");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppCategory",
                table: "Apps",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AppPlatform",
                table: "Apps",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
