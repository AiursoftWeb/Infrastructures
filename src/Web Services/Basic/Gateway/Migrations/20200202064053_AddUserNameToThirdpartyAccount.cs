using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Gateway.Migrations
{
    public partial class AddUserNameToThirdpartyAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ThirdPartyAccounts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ThirdPartyAccounts");
        }
    }
}
