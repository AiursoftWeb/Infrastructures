using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.EE.Migrations
{
    public partial class CreateWhatYouWillLearn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WhatYouWillLearn",
                table: "Courses",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WhatYouWillLearn",
                table: "Courses");
        }
    }
}
