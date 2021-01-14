using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Wrapgate.Migrations
{
    public partial class TestIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RecordUniqueName",
                table: "Records",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Records_RecordUniqueName",
                table: "Records",
                column: "RecordUniqueName",
                unique: true,
                filter: "[RecordUniqueName] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Records_RecordUniqueName",
                table: "Records");

            migrationBuilder.AlterColumn<string>(
                name: "RecordUniqueName",
                table: "Records",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
