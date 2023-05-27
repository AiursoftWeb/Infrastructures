using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Directory.Migrations
{
    public partial class RefactorImageStorageSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeadImgUrl",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "HeadImgFileKey",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeadImgFileKey",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "HeadImgUrl",
                table: "AspNetUsers",
                nullable: true);
        }
    }
}
