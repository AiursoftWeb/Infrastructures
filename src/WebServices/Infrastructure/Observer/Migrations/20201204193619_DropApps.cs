using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Observer.Migrations
{
    public partial class DropApps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ErrorLogs_ObserverApps_AppId",
                table: "ErrorLogs");

            migrationBuilder.DropTable(
                name: "ObserverApps");

            migrationBuilder.DropIndex(
                name: "IX_ErrorLogs_AppId",
                table: "ErrorLogs");

            migrationBuilder.AlterColumn<string>(
                name: "AppId",
                table: "ErrorLogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AppId",
                table: "ErrorLogs",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ObserverApps",
                columns: table => new
                {
                    AppId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObserverApps", x => x.AppId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ErrorLogs_AppId",
                table: "ErrorLogs",
                column: "AppId");

            migrationBuilder.AddForeignKey(
                name: "FK_ErrorLogs_ObserverApps_AppId",
                table: "ErrorLogs",
                column: "AppId",
                principalTable: "ObserverApps",
                principalColumn: "AppId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
