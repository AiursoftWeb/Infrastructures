using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Status.Migrations
{
    public partial class AddMonitorRule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MonitorRules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectName = table.Column<string>(nullable: true),
                    CheckAddress = table.Column<string>(nullable: true),
                    LastHealthStatus = table.Column<bool>(nullable: false),
                    ExpectedContent = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitorRules", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonitorRules");
        }
    }
}
