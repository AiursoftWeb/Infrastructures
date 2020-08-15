using Microsoft.EntityFrameworkCore.Migrations;

namespace Aiursoft.Wiki.Migrations
{
    public partial class CreateArticleAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArticleAddress",
                table: "Article",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArticleAddress",
                table: "Article");
        }
    }
}
