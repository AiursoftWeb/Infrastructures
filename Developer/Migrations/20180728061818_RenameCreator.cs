using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Developer.Migrations
{
    public partial class RenameCreator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Apps_AspNetUsers_CreaterId",
                table: "Apps");

            migrationBuilder.RenameColumn(
                name: "CreaterId",
                table: "Apps",
                newName: "CreatorId");

            migrationBuilder.RenameIndex(
                name: "IX_Apps_CreaterId",
                table: "Apps",
                newName: "IX_Apps_CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Apps_AspNetUsers_CreatorId",
                table: "Apps",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Apps_AspNetUsers_CreatorId",
                table: "Apps");

            migrationBuilder.RenameColumn(
                name: "CreatorId",
                table: "Apps",
                newName: "CreaterId");

            migrationBuilder.RenameIndex(
                name: "IX_Apps_CreatorId",
                table: "Apps",
                newName: "IX_Apps_CreaterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Apps_AspNetUsers_CreaterId",
                table: "Apps",
                column: "CreaterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
