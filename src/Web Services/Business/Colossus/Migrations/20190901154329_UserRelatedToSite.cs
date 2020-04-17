using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Colossus.Migrations
{
    public partial class UserRelatedToSite : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SiteName",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SiteType",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SiteName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "SiteType",
                table: "AspNetUsers");
        }
    }
}
