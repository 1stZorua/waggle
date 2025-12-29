using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Waggle.PostService.Migrations
{
    /// <inheritdoc />
    public partial class AddPostCompositeIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_posts_CreatedAt_Id",
                table: "posts",
                columns: new[] { "CreatedAt", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_posts_Id",
                table: "posts",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_posts_CreatedAt_Id",
                table: "posts");

            migrationBuilder.DropIndex(
                name: "IX_posts_Id",
                table: "posts");
        }
    }
}
