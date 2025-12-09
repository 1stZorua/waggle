using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Waggle.MediaService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "media",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UploaderId = table.Column<Guid>(type: "uuid", nullable: false),
                    BucketName = table.Column<string>(type: "character varying(63)", maxLength: 63, nullable: false),
                    ObjectName = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_media", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_media_BucketName",
                table: "media",
                column: "BucketName");

            migrationBuilder.CreateIndex(
                name: "IX_media_CreatedAt",
                table: "media",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_media_UploaderId",
                table: "media",
                column: "UploaderId");

            migrationBuilder.CreateIndex(
                name: "IX_media_UploaderId_CreatedAt",
                table: "media",
                columns: new[] { "UploaderId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "media");
        }
    }
}
