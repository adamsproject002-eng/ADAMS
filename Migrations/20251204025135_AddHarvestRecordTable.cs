using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADAMS.Migrations
{
    /// <inheritdoc />
    public partial class AddHarvestRecordTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HarvestRecord",
                columns: table => new
                {
                    HarvestRecordSN = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PondSN = table.Column<int>(type: "int", nullable: false),
                    FarmingCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HarvestDate = table.Column<DateTime>(type: "date", nullable: false),
                    HarvestType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    HarvestWeight = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    HarvestPCS = table.Column<int>(type: "int", nullable: false),
                    ABW = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DOC = table.Column<int>(type: "int", nullable: false),
                    ManageAccount = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_HarvestRecord", x => x.HarvestRecordSN);
                    table.ForeignKey(
                        name: "FK_HarvestRecord_Pond_PondSN",
                        column: x => x.PondSN,
                        principalTable: "Pond",
                        principalColumn: "PondSN",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HarvestRecord_PondSN",
                table: "HarvestRecord",
                column: "PondSN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HarvestRecord");
        }
    }
}
