using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Aiursoft.OSS.Migrations
{
    public partial class MigratedBackToSQLServer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Apps",
                columns: table => new
                {
                    AppId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apps", x => x.AppId);
                });

            migrationBuilder.CreateTable(
                name: "Bucket",
                columns: table => new
                {
                    BucketId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BelongingAppId = table.Column<string>(nullable: true),
                    BucketName = table.Column<string>(nullable: true),
                    OpenToRead = table.Column<bool>(nullable: false),
                    OpenToUpload = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bucket", x => x.BucketId);
                    table.ForeignKey(
                        name: "FK_Bucket_Apps_BelongingAppId",
                        column: x => x.BelongingAppId,
                        principalTable: "Apps",
                        principalColumn: "AppId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OSSFile",
                columns: table => new
                {
                    FileKey = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BucketId = table.Column<int>(nullable: false),
                    DownloadTimes = table.Column<int>(nullable: false),
                    FileExtension = table.Column<string>(nullable: true),
                    RealFileName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OSSFile", x => x.FileKey);
                    table.ForeignKey(
                        name: "FK_OSSFile_Bucket_BucketId",
                        column: x => x.BucketId,
                        principalTable: "Bucket",
                        principalColumn: "BucketId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Secrets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FileId = table.Column<int>(nullable: false),
                    UseTime = table.Column<DateTime>(nullable: false),
                    Used = table.Column<bool>(nullable: false),
                    UserIpAddress = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Secrets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Secrets_OSSFile_FileId",
                        column: x => x.FileId,
                        principalTable: "OSSFile",
                        principalColumn: "FileKey",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bucket_BelongingAppId",
                table: "Bucket",
                column: "BelongingAppId");

            migrationBuilder.CreateIndex(
                name: "IX_OSSFile_BucketId",
                table: "OSSFile",
                column: "BucketId");

            migrationBuilder.CreateIndex(
                name: "IX_Secrets_FileId",
                table: "Secrets",
                column: "FileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Secrets");

            migrationBuilder.DropTable(
                name: "OSSFile");

            migrationBuilder.DropTable(
                name: "Bucket");

            migrationBuilder.DropTable(
                name: "Apps");
        }
    }
}
