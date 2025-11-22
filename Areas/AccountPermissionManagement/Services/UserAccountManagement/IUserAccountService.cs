using ADAMS.Areas.AccountPermissionManagement.ViewModels.UserAccountManagement;

namespace ADAMS.Areas.AccountPermissionManagement.Services.UserAccountManagement
{
    public interface IUserAccountService
    {
        Task<UserAccountListViewModel> GetListViewModelAsync(
            int? tenantSN,
            string? keyword,
            int status,
            int page,
            int pageSize);

        Task<UserAccountEditViewModel> GetCreateViewModelAsync(int? accountSN);

        Task<UserAccountEditViewModel?> GetEditViewModelAsync(int accountSN);

        Task<UserAccountEditViewModel> RebuildEditViewModelAsync(UserAccountEditViewModel posted);

        Task CreateAsync(UserAccountEditViewModel model);

        Task UpdateAsync(UserAccountEditViewModel model);

        Task ToggleEnableAsync(int accountSN);

        Task SoftDeleteAsync(int accountSN);

        Task<List<AccountGroupOption>> GetAccountGroupsForTenantAsync(int tenantSN);
    }
}
