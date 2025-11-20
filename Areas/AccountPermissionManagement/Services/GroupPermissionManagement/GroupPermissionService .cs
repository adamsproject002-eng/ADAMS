using ADAMS.Areas.AccountPermissionManagement.Repositories.GroupPermissionManagement;
using ADAMS.Areas.AccountPermissionManagement.Services.FarmerManagement;
using ADAMS.Areas.AccountPermissionManagement.ViewModels.GroupPermissionManagement;
using ADAMS.Areas.Models;
using ADAMS.Data;
using ADAMS.Models;
using ADAMS.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ADAMS.Areas.AccountPermissionManagement.Services.GroupPermissionManagement
{
    public class GroupPermissionService : IGroupPermissionService
    {
        private readonly IGroupPermissionRepository _groupPermissionRepo;
        private readonly AppDbContext _context;
        private readonly ILogger<FarmerService> _logger;
        private readonly ICurrentAccountService _currentAccountService;

        public GroupPermissionService(
            IGroupPermissionRepository groupPermissionRepo,
            AppDbContext context,
            ILogger<FarmerService> logger,
            ICurrentAccountService currentAccountService)
        {
            _groupPermissionRepo = groupPermissionRepo;
            _context = context;
            _logger = logger;
            _currentAccountService = currentAccountService;
        }



        public async Task<GroupPermissionListViewModel> GetPagedListAsync(int tenantSN, string? groupName, int page, int pageSize)
        {
            bool isOp = _currentAccountService.IsOperationCompany;

            //取得Tenant下拉選單
            var tenantOptions = await _groupPermissionRepo
                .GetTenantOptionsAsync(isOp, _currentAccountService.TenantSN);

            //若前端送來的 tenantSN 不在可用範圍內，則為預設第一筆
            if (!tenantOptions.Any(t => t.SN == tenantSN))
                tenantSN = tenantOptions.First().SN;

            var (data, totalCount) = await _groupPermissionRepo.GetPagedListAsync(
                tenantSN, groupName, page, pageSize);

            var groupPermissions = new List<GroupPermissionViewModel>();

            foreach (var group in data)
            {
                groupPermissions.Add(new GroupPermissionViewModel
                {
                    AccountGroupSN = group.AccountGroupSN,
                    Name = group.Name,
                    IsDeleted = group.IsDeleted,
                    Remark = group.Remark,
                });
            }

            // 建立分頁資訊，包含搜索條件
            var pagination = new PaginationInfo
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                SearchFilters = new Dictionary<string, object>
                {
                    { "tenantSN", tenantSN  },
                    { "groupName", groupName ?? "" }
                }
            };

            return new GroupPermissionListViewModel
            {
                GroupPermissions = groupPermissions,
                Pagination = pagination,
                IsOperationCompany = _currentAccountService.IsOperationCompany,
                TenantOptions = tenantOptions,
                CurrentTenantSN = tenantSN,
            };
        }

        public async Task<GroupPermissionAccountListViewModel> GetAccountsByGroupAsync(int groupSn)
        {
            var accounts = await _groupPermissionRepo.GetAccountsByGroupAsync(groupSn);

            return new GroupPermissionAccountListViewModel
            {
                UserName = _currentAccountService.Name,
                Accounts = accounts,
            };
        }

        /// <summary>
        /// 取得某群組的所有功能權限
        /// </summary>
        public async Task<GroupPermissionDetailViewModel> GetPermissionsByGroupAsync(int accountGroupSN)
        {
            // 先抓群組
            var group = await _groupPermissionRepo.GetGroupAsync(accountGroupSN);

            if (group == null)
                throw new InvalidOperationException($"找不到指定的群組：AccountGroupSN = {accountGroupSN}");

            // 抓出所有要顯示在選單上的功能
            var allFunctions = await _groupPermissionRepo.GetAllMenuFunctionsAsync();
            // 抓出此群組已經勾選的功能 SN
            var grantedFunctionSNs = await _groupPermissionRepo.GetGrantedFunctionSNsAsync(accountGroupSN);

            var vm = new GroupPermissionDetailViewModel
            {
                AccountGroupSN = group.AccountGroupSN,
                GroupName = group.Name,
                Functions = allFunctions.Select(f => new FunctionPermissionItemViewModel
                {
                    FunctionSN = f.FunctionSN,
                    CName = f.CName,
                    FLevel = f.FLevel,
                    UpperFunctionSN = f.UpperFunctionSN,
                    Sort = f.Sort,
                    HasPermission = grantedFunctionSNs.Contains(f.FunctionSN)
                }).ToList()
            };

            return vm;
        }

        public async Task<GroupPermissionEditViewModel> GetCreateViewModelAsync()
        {
            var isOperationCompany = _currentAccountService.IsOperationCompany;
            var currentTenantSN = _currentAccountService.TenantSN;

            // 依照登入身分取得可用的養殖戶清單
            var tenantOptions = await _groupPermissionRepo.GetTenantOptionsAsync(
                isOperationCompany, currentTenantSN);

            if (!tenantOptions.Any())
                throw new InvalidOperationException("無可用的養殖戶可供選擇");

            var allFunctions = await _groupPermissionRepo.GetAllMenuFunctionsAsync();

            int tenantSN;
            string tenantName;

            if (isOperationCompany)
            {
                // 營運帳號：TenantSN 先給 0，由下拉選擇
                tenantSN = 0;
                tenantName = string.Empty;
            }
            else
            {
                // 非營運帳號：直接用目前登入者的 Tenant
                var t = tenantOptions.Single();
                tenantSN = t.SN;
                tenantName = t.TenantName ?? string.Empty;
            }

            return new GroupPermissionEditViewModel
            {
                AccountGroupSN = null,
                TenantSN = tenantSN,
                TenantName = tenantName,
                IsOperationCompany = isOperationCompany,
                TenantOptions = tenantOptions,
                Functions = allFunctions.Select(f => new FunctionPermissionEditItemViewModel
                {
                    FunctionSN = f.FunctionSN,
                    CName = f.CName,
                    FLevel = f.FLevel,
                    UpperFunctionSN = f.UpperFunctionSN,
                    Sort = f.Sort,
                    IsChecked = false
                }).ToList()
            };
        }

        public async Task<GroupPermissionEditViewModel> GetEditViewModelAsync(int accountGroupSN)
        {
            var group = await _groupPermissionRepo.GetGroupAsync(accountGroupSN)
                       ?? throw new InvalidOperationException($"找不到指定的群組：AccountGroupSN = {accountGroupSN}");

            var tenant = await _groupPermissionRepo.GetTenantAsync(group.TenantSN)
                         ?? throw new InvalidOperationException($"找不到養殖戶：TenantSN = {group.TenantSN}");

            var allFunctions = await _groupPermissionRepo.GetAllMenuFunctionsAsync();
            var grantedFunctionSNs = await _groupPermissionRepo.GetGrantedFunctionSNsAsync(accountGroupSN);

            var vm = new GroupPermissionEditViewModel
            {
                AccountGroupSN = group.AccountGroupSN,
                TenantSN = tenant.SN,
                TenantName = tenant.TenantName ?? string.Empty,
                Name = group.Name,
                Remark = group.Remark,
                IsOperationCompany = false,          // 修改畫面不需要下拉
                Functions = allFunctions.Select(f => new FunctionPermissionEditItemViewModel
                {
                    FunctionSN = f.FunctionSN,
                    CName = f.CName,
                    FLevel = f.FLevel,
                    UpperFunctionSN = f.UpperFunctionSN,
                    Sort = f.Sort,
                    IsChecked = grantedFunctionSNs.Contains(f.FunctionSN)
                }).ToList()
            };

            return vm;
        }

        // ======= 儲存（Create + Edit 共用）=======

        public async Task SaveGroupAsync(GroupPermissionEditViewModel model)
        {
            if (string.Equals(model.Name?.Trim(), "Admin", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("群組名稱不得為 Admin，請改用其他名稱。");
            }

            var isOperationCompany = _currentAccountService.IsOperationCompany;
            var currentTenantSN = _currentAccountService.TenantSN;
            var currentUser = string.IsNullOrWhiteSpace(_currentAccountService.Name)
                ? "system"
                : _currentAccountService.Name;

            // 非營運帳號：防呆，TenantSN 強制使用登入者的 Tenant
            if (!isOperationCompany)
            {
                model.TenantSN = currentTenantSN;
            }

            AccountGroup group;

            if (model.AccountGroupSN.HasValue)
            {
                // 修改
                group = await _groupPermissionRepo.GetGroupAsync(model.AccountGroupSN.Value)
                        ?? throw new InvalidOperationException($"找不到指定的群組：AccountGroupSN = {model.AccountGroupSN.Value}");

                group.Name = model.Name.Trim();
                group.Remark = model.Remark;
   
                group.TenantSN = model.TenantSN;

                group.ModifyTime = DateTime.Now;
                group.ModifyUser = currentUser;

                await _groupPermissionRepo.UpdateGroupAsync(group);
            }
            else
            {
                // 新增
                if (model.TenantSN == 0)
                {
                    throw new InvalidOperationException("請選擇養殖戶");
                }

                group = new AccountGroup
                {
                    TenantSN = model.TenantSN,
                    Name = model.Name.Trim(),
                    Remark = model.Remark,
                    IsDeleted = false,
                    CreateTime = DateTime.Now,
                    CreateUser = currentUser
                };

                await _groupPermissionRepo.AddGroupAsync(group);
                model.AccountGroupSN = group.AccountGroupSN;
            }

            var selectedFunctionSNs = model.Functions
                .Where(f => f.IsChecked)          
                .Select(f => f.FunctionSN)
                .ToList();

            await _groupPermissionRepo.ReplaceAuthorizationsAsync(group.AccountGroupSN, selectedFunctionSNs, _currentAccountService.Name);
        }

        public async Task DeleteGroupAsync(int accountGroupSN)
        {
            var group = await _groupPermissionRepo.GetGroupAsync(accountGroupSN)
                        ?? throw new InvalidOperationException("找不到指定的群組。");

            // 1. Admin 群組不可刪除（不加欄位的做法：用 Name == "Admin" 判斷）
            if (string.Equals(group.Name, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("預設 Admin 群組不可刪除。");
            }

            // 2. 確認沒有任何帳號歸屬於此群組
            var hasAccounts = await _groupPermissionRepo.HasAccountsAsync(accountGroupSN);
            if (hasAccounts)
            {
                throw new InvalidOperationException("刪除失敗：仍有使用者帳號歸屬於此群組，請先調整帳號群組。");
            }

            // 3. 軟刪除：只更新 IsDeleted / DeleteTime / DeleteUser，不真的移除資料
            var currentUser = _currentAccountService.Name ?? "system";

            group.IsDeleted = true;
            group.DeleteTime = DateTime.Now;
            group.DeleteUser = currentUser;

            await _groupPermissionRepo.UpdateGroupAsync(group);

            // 4.（可選）如果你希望 Authorization 也跟著軟刪除，可以加這段：
            await _groupPermissionRepo.SoftDeleteAuthorizationsByGroupAsync(accountGroupSN, currentUser);
        }
    }
}
