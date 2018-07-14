using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.EE.Migrations
{
    public partial class CreateSectionName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SectionName",
                table: "Section",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SectionName",
                table: "Section");
        }
    }
}
