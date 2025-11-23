using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADAMS.Migrations
{
    /// <inheritdoc />
    public partial class AddFryRecordForegnToFry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_FryRecord_FrySN",
                table: "FryRecord",
                column: "FrySN");

            migrationBuilder.AddForeignKey(
                name: "FK_FryRecord_Fry_FrySN",
                table: "FryRecord",
                column: "FrySN",
                principalTable: "Fry",
                principalColumn: "FrySN",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FryRecord_Fry_FrySN",
                table: "FryRecord");

            migrationBuilder.DropIndex(
                name: "IX_FryRecord_FrySN",
                table: "FryRecord");
        }
    }
}
