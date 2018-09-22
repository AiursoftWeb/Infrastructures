using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Developer.Migrations
{
    public partial class AddANewAuthority : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ChangeGrantInfo",
                table: "Apps",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangeGrantInfo",
                table: "Apps");
        }
    }
}
