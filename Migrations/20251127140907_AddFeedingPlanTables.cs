using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADAMS.Migrations
{
    /// <inheritdoc />
    public partial class AddFeedingPlanTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FeedingPlanMain",
                columns: table => new
                {
                    FPMSN = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantSN = table.Column<int>(type: "int", nullable: false),
                    FeedingPlanName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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
                    table.PrimaryKey("PK_FeedingPlanMain", x => x.FPMSN);
                    table.ForeignKey(
                        name: "FK_FeedingPlanMain_FishVariety_FVSN",
                        column: x => x.FVSN,
                        principalTable: "FishVariety",
                        principalColumn: "FVSN",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeedingPlanMain_Tenant_TenantSN",
                        column: x => x.TenantSN,
                        principalTable: "Tenant",
                        principalColumn: "SN",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeedingPlanDetail",
                columns: table => new
                {
                    FPDSN = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FPMSN = table.Column<int>(type: "int", nullable: false),
                    FP_DOC = table.Column<int>(type: "int", nullable: false),
                    FP_ABW = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    FP_FeedingRate = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
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
                    table.PrimaryKey("PK_FeedingPlanDetail", x => x.FPDSN);
                    table.ForeignKey(
                        name: "FK_FeedingPlanDetail_FeedingPlanMain_FPMSN",
                        column: x => x.FPMSN,
                        principalTable: "FeedingPlanMain",
                        principalColumn: "FPMSN",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeedingPlanDetail_FPMSN",
                table: "FeedingPlanDetail",
                column: "FPMSN");

            migrationBuilder.CreateIndex(
                name: "IX_FeedingPlanMain_FVSN",
                table: "FeedingPlanMain",
                column: "FVSN");

            migrationBuilder.CreateIndex(
                name: "IX_FeedingPlanMain_TenantSN",
                table: "FeedingPlanMain",
                column: "TenantSN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeedingPlanDetail");

            migrationBuilder.DropTable(
                name: "FeedingPlanMain");
        }
    }
}
