using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Account.Migrations
{
    public partial class addBio2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TowFAuthenticatorUri",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "TwoFACode",
                table: "AspNetUsers",
                newName: "Bio2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Bio2",
                table: "AspNetUsers",
                newName: "TwoFACode");

            migrationBuilder.AddColumn<string>(
                name: "TowFAuthenticatorUri",
                table: "AspNetUsers",
                nullable: true);
        }
    }
}
