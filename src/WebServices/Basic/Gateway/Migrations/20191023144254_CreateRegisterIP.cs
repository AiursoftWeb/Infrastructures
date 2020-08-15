using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Gateway.Migrations
{
    public partial class CreateRegisterIP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RegisterIPAddress",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegisterIPAddress",
                table: "AspNetUsers");
        }
    }
}
