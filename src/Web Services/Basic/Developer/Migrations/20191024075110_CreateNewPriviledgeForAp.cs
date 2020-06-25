using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Developer.Migrations
{
    // ReSharper disable once IdentifierTypo
    public partial class CreateNewPriviledgeForAp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ManageSocialAccount",
                table: "Apps",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ManageSocialAccount",
                table: "Apps");
        }
    }
}
