using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADAMS.Migrations
{
    /// <inheritdoc />
    public partial class InitSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Function",
                columns: table => new
                {
                    FunctionSN = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FLevel = table.Column<int>(type: "int", nullable: false),
                    UpperFunctionSN = table.Column<int>(type: "int", nullable: false),
                    Sort = table.Column<int>(type: "int", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsShow = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Function", x => x.FunctionSN);
                });

            migrationBuilder.CreateTable(
                name: "Tenant",
                columns: table => new
                {
                    SN = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantNum = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenantName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenantAddr = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ResponName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ResponPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ResponEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsEnable = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifyTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifyUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenant", x => x.SN);
                });

            migrationBuilder.CreateTable(
                name: "AccountGroup",
                columns: table => new
                {
                    AccountGroupSN = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenantSN = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_AccountGroup", x => x.AccountGroupSN);
                    table.ForeignKey(
                        name: "FK_AccountGroup_Tenant_TenantSN",
                        column: x => x.TenantSN,
                        principalTable: "Tenant",
                        principalColumn: "SN",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    AccountSN = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenantSN = table.Column<int>(type: "int", nullable: false),
                    AccGroupSN = table.Column<int>(type: "int", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsEnable = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModifyTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifyUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DeleteTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TenantSN1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.AccountSN);
                    table.ForeignKey(
                        name: "FK_Account_AccountGroup_AccGroupSN",
                        column: x => x.AccGroupSN,
                        principalTable: "AccountGroup",
                        principalColumn: "AccountGroupSN",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Account_Tenant_TenantSN",
                        column: x => x.TenantSN,
                        principalTable: "Tenant",
                        principalColumn: "SN",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Account_Tenant_TenantSN1",
                        column: x => x.TenantSN1,
                        principalTable: "Tenant",
                        principalColumn: "SN");
                });

            migrationBuilder.CreateTable(
                name: "Authorization",
                columns: table => new
                {
                    AuthorizationSN = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccGroupSN = table.Column<int>(type: "int", nullable: false),
                    FunctionSN = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_Authorization", x => x.AuthorizationSN);
                    table.ForeignKey(
                        name: "FK_Authorization_AccountGroup_AccGroupSN",
                        column: x => x.AccGroupSN,
                        principalTable: "AccountGroup",
                        principalColumn: "AccountGroupSN",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Authorization_Function_FunctionSN",
                        column: x => x.FunctionSN,
                        principalTable: "Function",
                        principalColumn: "FunctionSN",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_AccGroupSN",
                table: "Account",
                column: "AccGroupSN");

            migrationBuilder.CreateIndex(
                name: "IX_Account_TenantSN",
                table: "Account",
                column: "TenantSN");

            migrationBuilder.CreateIndex(
                name: "IX_Account_TenantSN1",
                table: "Account",
                column: "TenantSN1");

            migrationBuilder.CreateIndex(
                name: "IX_AccountGroup_TenantSN",
                table: "AccountGroup",
                column: "TenantSN");

            migrationBuilder.CreateIndex(
                name: "IX_Authorization_AccGroupSN",
                table: "Authorization",
                column: "AccGroupSN");

            migrationBuilder.CreateIndex(
                name: "IX_Authorization_FunctionSN",
                table: "Authorization",
                column: "FunctionSN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "Authorization");

            migrationBuilder.DropTable(
                name: "AccountGroup");

            migrationBuilder.DropTable(
                name: "Function");

            migrationBuilder.DropTable(
                name: "Tenant");
        }
    }
}
