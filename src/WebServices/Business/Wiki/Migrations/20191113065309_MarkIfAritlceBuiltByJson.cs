using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Wiki.Migrations
{
    // ReSharper disable once IdentifierTypo
    // ReSharper disable once UnusedType.Global
    public partial class MarkIfAritlceBuiltByJson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BuiltByJson",
                table: "Article",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuiltByJson",
                table: "Article");
        }
    }
}
