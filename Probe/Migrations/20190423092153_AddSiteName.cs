using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Probe.Migrations
{
    public partial class AddSiteName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SiteName",
                table: "Sites",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SiteName",
                table: "Sites");
        }
    }
}
