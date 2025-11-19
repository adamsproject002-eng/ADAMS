using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ADAMS.Migrations
{
    /// <inheritdoc />
    public partial class SeedSystemAccountGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                    SET IDENTITY_INSERT AccountGroup ON;
                    INSERT INTO AccountGroup (AccountGroupSN, Name, TenantSN, IsDeleted, CreateTime, CreateUser)
                    VALUES (0, N'總管理群組', 0, 0, GETDATE(), N'system');
                    SET IDENTITY_INSERT AccountGroup OFF;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                    DELETE FROM AccountGroup WHERE AccountGroupSN = 0;
            ");
        }
    }
}
