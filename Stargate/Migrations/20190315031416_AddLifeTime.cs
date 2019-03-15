using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Stargate.Migrations
{
    public partial class AddLifeTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "LifeTime",
                table: "Channels",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LifeTime",
                table: "Channels");
        }
    }
}
