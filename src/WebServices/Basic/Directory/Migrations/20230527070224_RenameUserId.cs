using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aiursoft.Directory.Migrations
{
    public partial class RenameUserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocalAppGrant_AspNetUsers_GatewayUserId",
                table: "LocalAppGrant");

            migrationBuilder.RenameColumn(
                name: "GatewayUserId",
                table: "LocalAppGrant",
                newName: "DirectoryUserId");

            migrationBuilder.RenameIndex(
                name: "IX_LocalAppGrant_GatewayUserId",
                table: "LocalAppGrant",
                newName: "IX_LocalAppGrant_DirectoryUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LocalAppGrant_AspNetUsers_DirectoryUserId",
                table: "LocalAppGrant",
                column: "DirectoryUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocalAppGrant_AspNetUsers_DirectoryUserId",
                table: "LocalAppGrant");

            migrationBuilder.RenameColumn(
                name: "DirectoryUserId",
                table: "LocalAppGrant",
                newName: "GatewayUserId");

            migrationBuilder.RenameIndex(
                name: "IX_LocalAppGrant_DirectoryUserId",
                table: "LocalAppGrant",
                newName: "IX_LocalAppGrant_GatewayUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LocalAppGrant_AspNetUsers_GatewayUserId",
                table: "LocalAppGrant",
                column: "GatewayUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
