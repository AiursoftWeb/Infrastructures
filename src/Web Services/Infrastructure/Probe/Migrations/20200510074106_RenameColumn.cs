using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Probe.Migrations
{
    public partial class RenameColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FolderId",
                newName: "RootFolderId",
                table: "Sites");

            migrationBuilder.RenameIndex(
                name: "IX_Sites_FolderId",
                newName: "IX_Sites_RootFolderId",
                table: "Sites");

            migrationBuilder.DropForeignKey(
                name: "FK_Sites_Folders_FolderId",
                table: "Sites");

            migrationBuilder.AddForeignKey(
                name: "FK_Sites_Folders_RootFolderId",
                table: "Sites",
                column: "RootFolderId",
                principalTable: "Folders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sites_Folders_RootFolderId",
                table: "Sites");

            migrationBuilder.DropIndex(
                name: "IX_Sites_RootFolderId",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "RootFolderId",
                table: "Sites");

            migrationBuilder.AddColumn<int>(
                name: "FolderId",
                table: "Sites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Sites_FolderId",
                table: "Sites",
                column: "FolderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sites_Folders_FolderId",
                table: "Sites",
                column: "FolderId",
                principalTable: "Folders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
