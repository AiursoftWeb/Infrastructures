using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Observer.Migrations
{
    public partial class SplitProjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ErrorLogs_StatusApps_AppId",
                table: "ErrorLogs");

            migrationBuilder.DropTable(
                name: "MonitorRules");

            migrationBuilder.DropTable(
                name: "StatusApps");

            migrationBuilder.CreateTable(
                name: "ObserverApps",
                columns: table => new
                {
                    AppId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObserverApps", x => x.AppId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ErrorLogs_ObserverApps_AppId",
                table: "ErrorLogs",
                column: "AppId",
                principalTable: "ObserverApps",
                principalColumn: "AppId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ErrorLogs_ObserverApps_AppId",
                table: "ErrorLogs");

            migrationBuilder.DropTable(
                name: "ObserverApps");

            migrationBuilder.CreateTable(
                name: "MonitorRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CheckAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpectedContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastCheckTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastHealthStatus = table.Column<bool>(type: "bit", nullable: false),
                    ProjectName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitorRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatusApps",
                columns: table => new
                {
                    AppId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusApps", x => x.AppId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ErrorLogs_StatusApps_AppId",
                table: "ErrorLogs",
                column: "AppId",
                principalTable: "StatusApps",
                principalColumn: "AppId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
