using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aiursoft.Directory.Migrations
{
    public partial class RenameATable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DirectoryAppInDb_AspNetUsers_CreatorId",
                table: "DirectoryAppInDb");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DirectoryAppInDb",
                table: "DirectoryAppInDb");

            migrationBuilder.RenameTable(
                name: "DirectoryAppInDb",
                newName: "DirectoryAppsInDb");

            migrationBuilder.RenameIndex(
                name: "IX_DirectoryAppInDb_CreatorId",
                table: "DirectoryAppsInDb",
                newName: "IX_DirectoryAppsInDb_CreatorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DirectoryAppsInDb",
                table: "DirectoryAppsInDb",
                column: "AppId");

            migrationBuilder.AddForeignKey(
                name: "FK_DirectoryAppsInDb_AspNetUsers_CreatorId",
                table: "DirectoryAppsInDb",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DirectoryAppsInDb_AspNetUsers_CreatorId",
                table: "DirectoryAppsInDb");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DirectoryAppsInDb",
                table: "DirectoryAppsInDb");

            migrationBuilder.RenameTable(
                name: "DirectoryAppsInDb",
                newName: "DirectoryAppInDb");

            migrationBuilder.RenameIndex(
                name: "IX_DirectoryAppsInDb_CreatorId",
                table: "DirectoryAppInDb",
                newName: "IX_DirectoryAppInDb_CreatorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DirectoryAppInDb",
                table: "DirectoryAppInDb",
                column: "AppId");

            migrationBuilder.AddForeignKey(
                name: "FK_DirectoryAppInDb_AspNetUsers_CreatorId",
                table: "DirectoryAppInDb",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
