using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADAMS.Migrations
{
    /// <inheritdoc />
    public partial class SeedSystemTenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                    SET IDENTITY_INSERT Tenant ON;
                    INSERT INTO Tenant (SN, TenantNum, TenantName, IsEnable, CreateTime, CreateUser)
                    VALUES (0, N'SYS', N'運營養殖戶', 1, GETDATE(), N'system');
                    SET IDENTITY_INSERT Tenant OFF;
                    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Tenant WHERE SN = 0");
        }
    }
}
