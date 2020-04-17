using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Developer.Migrations
{
    public partial class AddTrustedTag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TrustedApp",
                table: "Apps",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrustedApp",
                table: "Apps");
        }
    }
}
