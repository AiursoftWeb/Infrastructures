using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Probe.Migrations
{
    public partial class CreateBasicRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppId",
                table: "Sites",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FolderId",
                table: "Sites",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ContextId",
                table: "Folders",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContextId",
                table: "Files",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Sites_AppId",
                table: "Sites",
                column: "AppId");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_FolderId",
                table: "Sites",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_ContextId",
                table: "Folders",
                column: "ContextId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_ContextId",
                table: "Files",
                column: "ContextId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Folders_ContextId",
                table: "Files",
                column: "ContextId",
                principalTable: "Folders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Folders_Folders_ContextId",
                table: "Folders",
                column: "ContextId",
                principalTable: "Folders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sites_Apps_AppId",
                table: "Sites",
                column: "AppId",
                principalTable: "Apps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sites_Folders_FolderId",
                table: "Sites",
                column: "FolderId",
                principalTable: "Folders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Folders_ContextId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Folders_Folders_ContextId",
                table: "Folders");

            migrationBuilder.DropForeignKey(
                name: "FK_Sites_Apps_AppId",
                table: "Sites");

            migrationBuilder.DropForeignKey(
                name: "FK_Sites_Folders_FolderId",
                table: "Sites");

            migrationBuilder.DropIndex(
                name: "IX_Sites_AppId",
                table: "Sites");

            migrationBuilder.DropIndex(
                name: "IX_Sites_FolderId",
                table: "Sites");

            migrationBuilder.DropIndex(
                name: "IX_Folders_ContextId",
                table: "Folders");

            migrationBuilder.DropIndex(
                name: "IX_Files_ContextId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "AppId",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "FolderId",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "ContextId",
                table: "Folders");

            migrationBuilder.DropColumn(
                name: "ContextId",
                table: "Files");
        }
    }
}
