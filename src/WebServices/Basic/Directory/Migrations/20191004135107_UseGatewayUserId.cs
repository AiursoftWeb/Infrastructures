using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Directory.Migrations
{
    public partial class UseGatewayUserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocalAppGrant_AspNetUsers_APIUserId",
                table: "LocalAppGrant");

            migrationBuilder.DropIndex(
                name: "IX_LocalAppGrant_APIUserId",
                table: "LocalAppGrant");

            migrationBuilder.RenameColumn(
                name: "APIUserId",
                table: "LocalAppGrant",
                newName: "GatewayUserId");

            migrationBuilder.CreateIndex(
                name: "IX_LocalAppGrant_GatewayUserId",
                table: "LocalAppGrant",
                column: "GatewayUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LocalAppGrant_AspNetUsers_GatewayUserId",
                table: "LocalAppGrant",
                column: "GatewayUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocalAppGrant_AspNetUsers_GatewayUserId",
                table: "LocalAppGrant");

            migrationBuilder.DropIndex(
                name: "IX_LocalAppGrant_GatewayUserId",
                table: "LocalAppGrant");

            migrationBuilder.DropColumn(
                name: "GatewayUserId",
                table: "LocalAppGrant");

            migrationBuilder.AddColumn<string>(
                name: "APIUserId",
                table: "LocalAppGrant",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocalAppGrant_APIUserId",
                table: "LocalAppGrant",
                column: "APIUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LocalAppGrant_AspNetUsers_APIUserId",
                table: "LocalAppGrant",
                column: "APIUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
