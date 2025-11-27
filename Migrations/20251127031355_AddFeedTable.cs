using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADAMS.Migrations
{
    /// <inheritdoc />
    public partial class AddFeedTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Feed",
                columns: table => new
                {
                    FeedSN = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantSN = table.Column<int>(type: "int", nullable: false),
                    FeedName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SupplierSN = table.Column<int>(type: "int", nullable: false),
                    UnitName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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
                    table.PrimaryKey("PK_Feed", x => x.FeedSN);
                    table.ForeignKey(
                        name: "FK_Feed_Supplier_SupplierSN",
                        column: x => x.SupplierSN,
                        principalTable: "Supplier",
                        principalColumn: "SupplierSN",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Feed_Tenant_TenantSN",
                        column: x => x.TenantSN,
                        principalTable: "Tenant",
                        principalColumn: "SN",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Feed_SupplierSN",
                table: "Feed",
                column: "SupplierSN");

            migrationBuilder.CreateIndex(
                name: "IX_Feed_TenantSN",
                table: "Feed",
                column: "TenantSN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Feed");
        }
    }
}
