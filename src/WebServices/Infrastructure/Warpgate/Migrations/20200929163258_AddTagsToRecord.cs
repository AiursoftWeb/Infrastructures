using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Warpgate.Migrations
{
    public partial class AddTagsToRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Records",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Records");
        }
    }
}
