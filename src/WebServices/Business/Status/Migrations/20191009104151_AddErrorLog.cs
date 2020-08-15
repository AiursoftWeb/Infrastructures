using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Status.Migrations
{
    public partial class AddErrorLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EventLevel",
                table: "ErrorLogs",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventLevel",
                table: "ErrorLogs");
        }
    }
}
