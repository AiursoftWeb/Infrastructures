using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Account.Migrations
{
    public partial class AddTwoFAProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TowFAuthenticatorUri",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TwoFACode",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TwoFASharedKey",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TowFAuthenticatorUri",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TwoFACode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TwoFASharedKey",
                table: "AspNetUsers");
        }
    }
}
