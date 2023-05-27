using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Directory.Migrations
{
    // ReSharper disable once UnusedType.Global
    // ReSharper disable once IdentifierTypo
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
