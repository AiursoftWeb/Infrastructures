using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.EE.Migrations
{
    public partial class CreateSection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chapters_Courses_CourseId",
                table: "Chapters");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "Chapters",
                newName: "SectionId");

            migrationBuilder.RenameIndex(
                name: "IX_Chapters_CourseId",
                table: "Chapters",
                newName: "IX_Chapters_SectionId");

            migrationBuilder.CreateTable(
                name: "Section",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CourseId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Section", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Section_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Section_CourseId",
                table: "Section",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chapters_Section_SectionId",
                table: "Chapters",
                column: "SectionId",
                principalTable: "Section",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chapters_Section_SectionId",
                table: "Chapters");

            migrationBuilder.DropTable(
                name: "Section");

            migrationBuilder.RenameColumn(
                name: "SectionId",
                table: "Chapters",
                newName: "CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_Chapters_SectionId",
                table: "Chapters",
                newName: "IX_Chapters_CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chapters_Courses_CourseId",
                table: "Chapters",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
