using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Waggle.CommentService.Migrations
{
    /// <inheritdoc />
    public partial class RenameParentCommentIdToParentId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ParentCommentId",
                table: "comments",
                newName: "ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_comments_PostId_ParentCommentId",
                table: "comments",
                newName: "IX_comments_PostId_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_comments_ParentCommentId",
                table: "comments",
                newName: "IX_comments_ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ParentId",
                table: "comments",
                newName: "ParentCommentId");

            migrationBuilder.RenameIndex(
                name: "IX_comments_PostId_ParentId",
                table: "comments",
                newName: "IX_comments_PostId_ParentCommentId");

            migrationBuilder.RenameIndex(
                name: "IX_comments_ParentId",
                table: "comments",
                newName: "IX_comments_ParentCommentId");
        }
    }
}
