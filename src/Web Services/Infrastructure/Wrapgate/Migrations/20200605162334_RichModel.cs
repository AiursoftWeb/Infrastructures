using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Wrapgate.Migrations
{
    public partial class RichModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RecordUniqueName",
                table: "Records",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetUrl",
                table: "Records",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Records",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecordUniqueName",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "TargetUrl",
                table: "Records");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Records");
        }
    }
}
