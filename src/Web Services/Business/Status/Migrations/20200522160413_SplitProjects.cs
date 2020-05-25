using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Aiursoft.Status.Migrations
{
    public partial class SplitProjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ErrorLogs");

            migrationBuilder.DropTable(
                name: "StatusApps");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "ErrorLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    EventLevel = table.Column<int>(type: "int", nullable: false),
                    LogTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StackTrace = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ErrorLogs_StatusApps_AppId",
                        column: x => x.AppId,
                        principalTable: "StatusApps",
                        principalColumn: "AppId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ErrorLogs_AppId",
                table: "ErrorLogs",
                column: "AppId");
        }
    }
}
