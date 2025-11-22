using ADAMS.Areas.AccountPermissionManagement.ViewModels.UserAccountManagement;
using ADAMS.Areas.Models;
using ADAMS.Models;

namespace ADAMS.Areas.AccountPermissionManagement.Repositories.UserAccountManagement

{
    public interface IUserAccountRepository
    {
        Task<(List<Account> Data, int TotalCount)> GetPagedListAsync(
            int tenantSN,
            string? keyword,
            int status,      // 0=全部,1=啟用,2=停用
            int page,
            int pageSize);

        Task<List<TenantOption>> GetTenantOptionsAsync(bool isOperationCompany, int currentTenantSN);

        Task<List<AccountGroupOption>> GetAccountGroupOptionsAsync(int tenantSN);

        Task<Account?> GetAccountAsync(int accountSN);

        Task<bool> IsAccountNameDuplicateAsync(int tenantSN, string accountName, int? excludeAccountSN = null);

        Task AddAccountAsync(Account account);

        Task UpdateAccountAsync(Account account);

        Task SaveChangesAsync();
    }
}
