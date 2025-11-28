using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADAMS.Migrations
{
    /// <inheritdoc />
    public partial class AddGrowTargetTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GrowTargetMain",
                columns: table => new
                {
                    GTMSN = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GTMName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FVSN = table.Column<int>(type: "int", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_GrowTargetMain", x => x.GTMSN);
                    table.ForeignKey(
                        name: "FK_GrowTargetMain_FishVariety_FVSN",
                        column: x => x.FVSN,
                        principalTable: "FishVariety",
                        principalColumn: "FVSN",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GrowTargetDetail",
                columns: table => new
                {
                    GTDSN = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GTMSN = table.Column<int>(type: "int", nullable: false),
                    GT_DOC = table.Column<int>(type: "int", nullable: false),
                    GT_ABW = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    GT_DailyGrow = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_GrowTargetDetail", x => x.GTDSN);
                    table.ForeignKey(
                        name: "FK_GrowTargetDetail_GrowTargetMain_GTMSN",
                        column: x => x.GTMSN,
                        principalTable: "GrowTargetMain",
                        principalColumn: "GTMSN",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GrowTargetDetail_GTMSN",
                table: "GrowTargetDetail",
                column: "GTMSN");

            migrationBuilder.CreateIndex(
                name: "IX_GrowTargetMain_FVSN",
                table: "GrowTargetMain",
                column: "FVSN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GrowTargetDetail");

            migrationBuilder.DropTable(
                name: "GrowTargetMain");
        }
    }
}
