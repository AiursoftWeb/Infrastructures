using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Stargate.Migrations
{
    public partial class DropLifeTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LifeTime",
                table: "Channels");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "LifeTime",
                table: "Channels",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }
    }
}
