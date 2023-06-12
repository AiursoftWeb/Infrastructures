using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aiursoft.Directory.Migrations
{
    public partial class AddDirectoryAppInDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DirectoryAppInDb",
                columns: table => new
                {
                    AppId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AppSecret = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IconPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppCreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EnableOAuth = table.Column<bool>(type: "bit", nullable: false),
                    ForceInputPassword = table.Column<bool>(type: "bit", nullable: false),
                    ForceConfirmation = table.Column<bool>(type: "bit", nullable: false),
                    DebugMode = table.Column<bool>(type: "bit", nullable: false),
                    AppDomain = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppFailCallbackUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ViewOpenId = table.Column<bool>(type: "bit", nullable: false),
                    ViewPhoneNumber = table.Column<bool>(type: "bit", nullable: false),
                    ChangePhoneNumber = table.Column<bool>(type: "bit", nullable: false),
                    ConfirmEmail = table.Column<bool>(type: "bit", nullable: false),
                    ChangeBasicInfo = table.Column<bool>(type: "bit", nullable: false),
                    ChangePassword = table.Column<bool>(type: "bit", nullable: false),
                    ChangeGrantInfo = table.Column<bool>(type: "bit", nullable: false),
                    ViewAuditLog = table.Column<bool>(type: "bit", nullable: false),
                    ManageSocialAccount = table.Column<bool>(type: "bit", nullable: false),
                    TrustedApp = table.Column<bool>(type: "bit", nullable: false),
                    PrivacyStatementUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LicenseUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectoryAppInDb", x => x.AppId);
                    table.ForeignKey(
                        name: "FK_DirectoryAppInDb_AspNetUsers_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DirectoryAppInDb_CreatorId",
                table: "DirectoryAppInDb",
                column: "CreatorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DirectoryAppInDb");
        }
    }
}
