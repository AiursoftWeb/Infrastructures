using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Probe.Migrations
{
    public partial class FixFileNameIsInt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "Files",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "FileName",
                table: "Files",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
