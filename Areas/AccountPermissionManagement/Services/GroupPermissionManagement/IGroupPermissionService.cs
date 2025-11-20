using ADAMS.Areas.AccountPermissionManagement.ViewModels.GroupPermissionManagement;

namespace ADAMS.Areas.AccountPermissionManagement.Services.GroupPermissionManagement
{
    public interface IGroupPermissionService
    {
        Task<GroupPermissionAccountListViewModel> GetAccountsByGroupAsync(int groupSn);
        Task<GroupPermissionListViewModel> GetPagedListAsync(int tenantSN, string? groupName, int page, int pageSize);

        Task<GroupPermissionDetailViewModel> GetPermissionsByGroupAsync(int accountGroupSN);

        // 新增畫面（Create）：登入者資訊由 ICurrentAccountService 判斷
        Task<GroupPermissionEditViewModel> GetCreateViewModelAsync();

        Task<GroupPermissionEditViewModel> GetEditViewModelAsync(int accountGroupSN);

        // 儲存（包含新增 / 修改群組 + 權限）
        Task SaveGroupAsync(GroupPermissionEditViewModel model);

        Task DeleteGroupAsync(int accountGroupSN);
    }
}
