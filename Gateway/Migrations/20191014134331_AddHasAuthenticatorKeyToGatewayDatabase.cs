using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Gateway.Migrations
{
    public partial class AddHasAuthenticatorKeyToGatewayDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasAuthenticator",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TwoFAKey",
                table: "AspNetUsers",
                type: "bool",
                nullable: false);
        }
    }
}
