using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Probe.Migrations
{
    public partial class AddIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SiteName",
                table: "Sites",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sites_SiteName",
                table: "Sites",
                column: "SiteName",
                unique: true,
                filter: "[SiteName] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sites_SiteName",
                table: "Sites");

            migrationBuilder.AlterColumn<string>(
                name: "SiteName",
                table: "Sites",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
