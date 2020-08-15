using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.WWW.Migrations
{
    public partial class DropOldUserIconId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeadImgFileKey",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HeadImgFileKey",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);
        }
    }
}
