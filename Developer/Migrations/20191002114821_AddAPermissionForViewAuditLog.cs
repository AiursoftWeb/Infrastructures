using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Developer.Migrations
{
    public partial class AddAPermissionForViewAuditLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ViewAuditLog",
                table: "Apps",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ViewAuditLog",
                table: "Apps");
        }
    }
}
