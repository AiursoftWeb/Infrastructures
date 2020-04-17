using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Gateway.Migrations
{
    public partial class CreateHas2FAKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Has2FAKey",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Has2FAKey",
                table: "AspNetUsers");
        }
    }
}
