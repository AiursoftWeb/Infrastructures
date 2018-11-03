using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Colossus.Migrations
{
    public partial class CreateSourceNameForColossus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SourceFileName",
                table: "UploadRecords",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceFileName",
                table: "UploadRecords");
        }
    }
}
