using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Developer.Migrations
{
    public partial class AddUserIconPathBasedOnProbe : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IconFilePath",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconFilePath",
                table: "AspNetUsers");
        }
    }
}
