using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.OSS.Migrations
{
    public partial class SupportMultiTimesSecrets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Secrets", true);

            migrationBuilder.DropColumn(
                name: "Used",
                table: "Secrets");

            migrationBuilder.AddColumn<int>(
                name: "MaxUseTime",
                table: "Secrets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UsedTimes",
                table: "Secrets",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxUseTime",
                table: "Secrets");

            migrationBuilder.DropColumn(
                name: "UsedTimes",
                table: "Secrets");

            migrationBuilder.AddColumn<bool>(
                name: "Used",
                table: "Secrets",
                nullable: false,
                defaultValue: false);
        }
    }
}
