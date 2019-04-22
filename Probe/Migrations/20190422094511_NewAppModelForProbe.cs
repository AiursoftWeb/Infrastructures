using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Probe.Migrations
{
    public partial class NewAppModelForProbe : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sites_Apps_AppId",
                table: "Sites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Apps",
                table: "Apps");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Apps");

            migrationBuilder.AlterColumn<string>(
                name: "AppId",
                table: "Sites",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "FolderName",
                table: "Folders",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FileName",
                table: "Files",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadTime",
                table: "Files",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "AppId",
                table: "Apps",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Apps",
                table: "Apps",
                column: "AppId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sites_Apps_AppId",
                table: "Sites",
                column: "AppId",
                principalTable: "Apps",
                principalColumn: "AppId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sites_Apps_AppId",
                table: "Sites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Apps",
                table: "Apps");

            migrationBuilder.DropColumn(
                name: "FolderName",
                table: "Folders");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "UploadTime",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "AppId",
                table: "Apps");

            migrationBuilder.AlterColumn<int>(
                name: "AppId",
                table: "Sites",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Apps",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Apps",
                table: "Apps",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sites_Apps_AppId",
                table: "Sites",
                column: "AppId",
                principalTable: "Apps",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
