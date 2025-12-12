using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADAMS.Migrations
{
    /// <inheritdoc />
    public partial class AddFeedingRecordTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FeedingRecord",
                columns: table => new
                {
                    FeedingRecordSN = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PondSN = table.Column<int>(type: "int", nullable: false),
                    FarmingCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FeedingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeZoneSN = table.Column<int>(type: "int", nullable: false),
                    FeedSN = table.Column<int>(type: "int", nullable: false),
                    FeedingAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SurvivalRate = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    ABW = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DOC = table.Column<int>(type: "int", nullable: false),
                    ManageAccount = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifyTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifyUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedingRecord", x => x.FeedingRecordSN);
                    table.ForeignKey(
                        name: "FK_FeedingRecord_Feed_FeedSN",
                        column: x => x.FeedSN,
                        principalTable: "Feed",
                        principalColumn: "FeedSN",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeedingRecord_Pond_PondSN",
                        column: x => x.PondSN,
                        principalTable: "Pond",
                        principalColumn: "PondSN",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeedingRecord_TimeZone_TimeZoneSN",
                        column: x => x.TimeZoneSN,
                        principalTable: "TimeZone",
                        principalColumn: "TimeZoneSN",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeedingRecord_FeedSN",
                table: "FeedingRecord",
                column: "FeedSN");

            migrationBuilder.CreateIndex(
                name: "IX_FeedingRecord_PondSN",
                table: "FeedingRecord",
                column: "PondSN");

            migrationBuilder.CreateIndex(
                name: "IX_FeedingRecord_TimeZoneSN",
                table: "FeedingRecord",
                column: "TimeZoneSN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeedingRecord");
        }
    }
}
