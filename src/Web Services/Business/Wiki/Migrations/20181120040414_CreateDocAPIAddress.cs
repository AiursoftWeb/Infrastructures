using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Wiki.Migrations
{
    public partial class CreateDocAPIAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocAPIAddress",
                table: "Collections",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocAPIAddress",
                table: "Collections");
        }
    }
}
