using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADAMS.Migrations
{
    /// <inheritdoc />
    public partial class AddFryRecordTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FryRecord",
                columns: table => new
                {
                    FryRecordSN = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PondSN = table.Column<int>(type: "int", nullable: false),
                    FarmingNum = table.Column<int>(type: "int", nullable: false),
                    FarmingCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FrySN = table.Column<int>(type: "int", nullable: false),
                    FarmingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FarmingPCS = table.Column<int>(type: "int", nullable: false),
                    FryAge = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PondArea = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    FarmingDensity = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ManageAccount = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TTL_Weight = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    TTL_PCS = table.Column<int>(type: "int", nullable: true),
                    SurvivalRate = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
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
                    table.PrimaryKey("PK_FryRecord", x => x.FryRecordSN);
                    table.ForeignKey(
                        name: "FK_FryRecord_Fry_FrySN",
                        column: x => x.FrySN,
                        principalTable: "Fry",
                        principalColumn: "FrySN",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FryRecord_Pond_PondSN",
                        column: x => x.PondSN,
                        principalTable: "Pond",
                        principalColumn: "PondSN",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FryRecord_FrySN",
                table: "FryRecord",
                column: "FrySN");

            migrationBuilder.CreateIndex(
                name: "IX_FryRecord_PondSN",
                table: "FryRecord",
                column: "PondSN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FryRecord");
        }
    }
}
