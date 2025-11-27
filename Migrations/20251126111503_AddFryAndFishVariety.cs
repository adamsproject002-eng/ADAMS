using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADAMS.Migrations
{
    /// <inheritdoc />
    public partial class AddFryAndFishVariety : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FishVariety",
                columns: table => new
                {
                    FVSN = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FVName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
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
                    table.PrimaryKey("PK_FishVariety", x => x.FVSN);
                });

            migrationBuilder.CreateTable(
                name: "Fry",
                columns: table => new
                {
                    FrySN = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantSN = table.Column<int>(type: "int", nullable: false),
                    FryName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SupplierSN = table.Column<int>(type: "int", nullable: false),
                    FVSN = table.Column<int>(type: "int", nullable: false),
                    UnitName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_Fry", x => x.FrySN);
                    table.ForeignKey(
                        name: "FK_Fry_FishVariety_FVSN",
                        column: x => x.FVSN,
                        principalTable: "FishVariety",
                        principalColumn: "FVSN",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fry_Supplier_SupplierSN",
                        column: x => x.SupplierSN,
                        principalTable: "Supplier",
                        principalColumn: "SupplierSN",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fry_Tenant_TenantSN",
                        column: x => x.TenantSN,
                        principalTable: "Tenant",
                        principalColumn: "SN",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fry_FVSN",
                table: "Fry",
                column: "FVSN");

            migrationBuilder.CreateIndex(
                name: "IX_Fry_SupplierSN",
                table: "Fry",
                column: "SupplierSN");

            migrationBuilder.CreateIndex(
                name: "IX_Fry_TenantSN",
                table: "Fry",
                column: "TenantSN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Fry");

            migrationBuilder.DropTable(
                name: "FishVariety");
        }
    }
}
