using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace flashCards.Migrations
{
    public partial class duplicates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_FlashcardPacks_PackName",
                table: "FlashcardPacks",
                column: "PackName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FlashcardPacks_PackName",
                table: "FlashcardPacks");
        }
    }
}
