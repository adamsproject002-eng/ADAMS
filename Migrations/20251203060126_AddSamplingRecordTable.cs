using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADAMS.Migrations
{
    /// <inheritdoc />
    public partial class AddSamplingRecordTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SamplingRecord",
                columns: table => new
                {
                    SamplingRecordSN = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PondSN = table.Column<int>(type: "int", nullable: false),
                    FarmingCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SamplingDate = table.Column<DateTime>(type: "date", nullable: false),
                    SamplingType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SamplingWeight = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SamplingPCS = table.Column<int>(type: "int", nullable: false),
                    ABW = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DOC = table.Column<int>(type: "int", nullable: false),
                    ManageAccount = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifyTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifyUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SamplingRecord", x => x.SamplingRecordSN);
                    table.ForeignKey(
                        name: "FK_SamplingRecord_Pond_PondSN",
                        column: x => x.PondSN,
                        principalTable: "Pond",
                        principalColumn: "PondSN",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SamplingRecord_PondSN",
                table: "SamplingRecord",
                column: "PondSN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SamplingRecord");
        }
    }
}
