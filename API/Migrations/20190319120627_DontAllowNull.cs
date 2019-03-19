using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.API.Migrations
{
    public partial class DontAllowNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Priority",
                table: "UserEmails",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Priority",
                table: "UserEmails",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
