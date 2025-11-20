using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADAMS.Migrations
{
    /// <inheritdoc />
    public partial class AddFryRecordPondRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Area",
                columns: table => new
                {
                    AreaSN = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenantSN = table.Column<int>(type: "int", nullable: false),
                    GPSX = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    GPSY = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModityTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifyUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Area", x => x.AreaSN);
                });

            migrationBuilder.CreateTable(
                name: "Pond",
                columns: table => new
                {
                    PondSN = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaSN = table.Column<int>(type: "int", nullable: false),
                    TenantSN = table.Column<int>(type: "int", nullable: false),
                    PondWidth = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PondLength = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PondArea = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    GPSX = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    GPSY = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModityTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifyUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pond", x => x.PondSN);
                    table.ForeignKey(
                        name: "FK_Pond_Area_AreaSN",
                        column: x => x.AreaSN,
                        principalTable: "Area",
                        principalColumn: "AreaSN",
                        onDelete: ReferentialAction.Restrict);
                });

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
                    ModityTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifyUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FryRecord", x => x.FryRecordSN);
                    table.ForeignKey(
                        name: "FK_FryRecord_Pond_PondSN",
                        column: x => x.PondSN,
                        principalTable: "Pond",
                        principalColumn: "PondSN",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FryRecord_PondSN",
                table: "FryRecord",
                column: "PondSN");

            migrationBuilder.CreateIndex(
                name: "IX_Pond_AreaSN",
                table: "Pond",
                column: "AreaSN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FryRecord");

            migrationBuilder.DropTable(
                name: "Pond");

            migrationBuilder.DropTable(
                name: "Area");
        }
    }
}
