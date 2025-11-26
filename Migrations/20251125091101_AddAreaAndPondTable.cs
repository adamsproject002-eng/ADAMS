using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADAMS.Migrations
{
    /// <inheritdoc />
    public partial class AddAreaAndPondTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_Tenant_TenantSN1",
                table: "Account");

            migrationBuilder.DropIndex(
                name: "IX_Account_TenantSN1",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "TenantSN1",
                table: "Account");

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
                    ModifyTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifyUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Area", x => x.AreaSN);
                    table.ForeignKey(
                        name: "FK_Area_Tenant_TenantSN",
                        column: x => x.TenantSN,
                        principalTable: "Tenant",
                        principalColumn: "SN",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pond",
                columns: table => new
                {
                    PondSN = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaSN = table.Column<int>(type: "int", nullable: false),
                    TenantSN = table.Column<int>(type: "int", nullable: false),
                    PondNum = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PondWidth = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PondLength = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PondArea = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    GPSX = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    GPSY = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
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
                    table.PrimaryKey("PK_Pond", x => x.PondSN);
                    table.ForeignKey(
                        name: "FK_Pond_Area_AreaSN",
                        column: x => x.AreaSN,
                        principalTable: "Area",
                        principalColumn: "AreaSN",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pond_Tenant_TenantSN",
                        column: x => x.TenantSN,
                        principalTable: "Tenant",
                        principalColumn: "SN",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Area_TenantSN",
                table: "Area",
                column: "TenantSN");

            migrationBuilder.CreateIndex(
                name: "IX_Pond_AreaSN",
                table: "Pond",
                column: "AreaSN");

            migrationBuilder.CreateIndex(
                name: "IX_Pond_TenantSN",
                table: "Pond",
                column: "TenantSN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pond");

            migrationBuilder.DropTable(
                name: "Area");

            migrationBuilder.AddColumn<int>(
                name: "TenantSN1",
                table: "Account",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Account_TenantSN1",
                table: "Account",
                column: "TenantSN1");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_Tenant_TenantSN1",
                table: "Account",
                column: "TenantSN1",
                principalTable: "Tenant",
                principalColumn: "SN");
        }
    }
}
