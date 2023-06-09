using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TagImages.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixImagesTagMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageTag");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImageTag",
                columns: table => new
                {
                    ImagesId = table.Column<int>(type: "int", nullable: false),
                    TagsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageTag", x => new { x.ImagesId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_ImageTag_Image_ImagesId",
                        column: x => x.ImagesId,
                        principalTable: "Image",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImageTag_Tag_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImageTag_TagsId",
                table: "ImageTag",
                column: "TagsId");
        }
    }
}
