using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.EE.Migrations
{
    public partial class CreateLongDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LongDescription",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LongDescription",
                table: "AspNetUsers");
        }
    }
}
