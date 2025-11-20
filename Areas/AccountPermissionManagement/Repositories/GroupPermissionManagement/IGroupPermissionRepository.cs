using ADAMS.Areas.AccountPermissionManagement.ViewModels.GroupPermissionManagement;
using ADAMS.Models;

namespace ADAMS.Areas.AccountPermissionManagement.Repositories.GroupPermissionManagement
{
    public interface IGroupPermissionRepository
    {
        Task<List<Account>> GetAccountsByGroupAsync(int groupSn);
        Task<(List<AccountGroup> Data, int TotalCount)> GetPagedListAsync(
            int tenantSN,
            string? groupName,
            int page,
            int pageSize);

        Task<List<TenantOption>> GetTenantOptionsAsync(
            bool isOperationCompany,
            int currentTenantSN);

        Task<AccountGroup?> GetGroupAsync(int accountGroupSN);
        Task<List<Function>> GetAllMenuFunctionsAsync();
        Task<List<int>> GetGrantedFunctionSNsAsync(int accountGroupSN);

        Task<Tenant?> GetTenantAsync(int tenantSN);

        Task AddGroupAsync(AccountGroup group);

        Task UpdateGroupAsync(AccountGroup group);

        /// <summary>
        /// 將某群組先刪除舊的再新增新的
        /// </summary>
        Task ReplaceAuthorizationsAsync(int accountGroupSN, IEnumerable<int> functionSNs , string? currentAccount);

        Task<bool> HasAccountsAsync(int accountGroupSN);
        Task SoftDeleteAuthorizationsByGroupAsync(int accountGroupSN, string currentUser);
    }
}
