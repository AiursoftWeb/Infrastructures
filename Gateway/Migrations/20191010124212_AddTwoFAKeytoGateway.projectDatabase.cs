using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Gateway.Migrations
{
    public partial class AddTwoFAKeytoGatewayprojectDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TwoFAKey",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TwoFAKey",
                table: "AspNetUsers");
        }
    }
}
