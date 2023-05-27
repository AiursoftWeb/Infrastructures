using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Directory.Migrations
{
    public partial class RenameGrant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AppID",
                table: "LocalAppGrant",
                newName: "AppId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AppId",
                table: "LocalAppGrant",
                newName: "AppID");
        }
    }
}
