using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Developer.Migrations
{
    public partial class RemoveAppIconAddressAndUseProbeToSaveAppIcons : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AppIconAddress",
                table: "Apps",
                newName: "IconPath");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IconPath",
                table: "Apps",
                newName: "AppIconAddress");
        }
    }
}
