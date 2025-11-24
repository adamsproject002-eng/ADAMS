using ADAMS.Areas.AccountPermissionManagement.Repositories.UserAccountManagement;
using ADAMS.Areas.AccountPermissionManagement.ViewModels.UserAccountManagement;
using ADAMS.Areas.Models;
using ADAMS.Helpers;
using ADAMS.Models;
using ADAMS.Services;

namespace ADAMS.Areas.AccountPermissionManagement.Services.UserAccountManagement
{
    public class UserAccountService : IUserAccountService
    {
        private readonly IUserAccountRepository _repo;
        private readonly ICurrentAccountService _current;

        public UserAccountService(
            IUserAccountRepository repo,
            ICurrentAccountService current)
        {
            _repo = repo;
            _current = current;
        }

        private (bool isOperationCompany, int tenantSN) GetTenantContext(int? fromQuery)
        {
            if (_current.IsOperationCompany)
            {
                // 營運公司：可切換養殖戶，預設使用 QueryString 的 tenantSN，沒有就用 1 或第一個
                var tenantSN = fromQuery ?? _current.TenantSN;
                return (true, tenantSN);
            }
            else
            {
                // 養殖戶：只能看自己
                return (false, _current.TenantSN);
            }
        }

        public async Task<UserAccountListViewModel> GetListViewModelAsync(
            int? tenantSN,
            string? keyword,
            int status,
            int page,
            int pageSize)
        {
            var (isOp, currentTenantSN) = GetTenantContext(tenantSN);

            // 養殖戶下拉
            var tenantOptions = await _repo.GetTenantOptionsAsync(isOp, currentTenantSN);

            // 預設顯示第一筆或當前
            if (currentTenantSN == 0 && tenantOptions.Any())
            {
                currentTenantSN = tenantOptions.First().SN;
            }

            // 資料
            var (data, totalCount) =
                await _repo.GetPagedListAsync(currentTenantSN, keyword, status, page, pageSize);

            var items = data.Select(a => new UserAccountListItemViewModel
            {
                AccountSN = a.AccountSN,
                AccountName = a.AccountName,
                GroupName = a.AccGroup?.Name ?? string.Empty,
                RealName = a.RealName,
                Email = a.Email,
                IsEnable = a.IsEnable,
                Remark = a.Remark,
                IsDefaultAccount = string.Equals(a.AccountName, "Admin", StringComparison.OrdinalIgnoreCase)
            }).ToList();

            var pagination = new PaginationInfo
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                SearchFilters = new Dictionary<string, object>
                {
                    { "tenantSN", tenantSN ?? 1 },
                    { "keyword", keyword ?? "" },
                    { "status" , status}
                }
            };

            var vm = new UserAccountListViewModel
            {
                IsOperationCompany = isOp,
                CurrentTenantSN = currentTenantSN,
                TenantOptions = tenantOptions,
                Keyword = keyword,
                Status = status,
                Accounts = items,
                Pagination = pagination
            };

            return vm;
        }

        public async Task<UserAccountEditViewModel> GetCreateViewModelAsync(int? tenantSNFromQuery = null)
        {
            var (isOp, currentTenantSN) = GetTenantContext(tenantSNFromQuery);

            var tenantOptions = await _repo.GetTenantOptionsAsync(isOp, currentTenantSN);

            if ((currentTenantSN == 0 || !tenantOptions.Any(t => t.SN == currentTenantSN))
                && tenantOptions.Any())
            {
                currentTenantSN = tenantOptions.First().SN;
            }

            var currentTenant = tenantOptions.FirstOrDefault(t => t.SN == currentTenantSN);
            var tenantName = currentTenant?.TenantName ?? string.Empty;

            var groupOptions = await _repo.GetAccountGroupOptionsAsync(currentTenantSN);

            return new UserAccountEditViewModel
            {
                AccountSN = null,
                AccountName = string.Empty,
                TenantSN = currentTenantSN,
                TenantName = tenantName,
                IsEnable = true,
                IsDefaultAccount = false,
                IsOperationCompany = isOp,
                TenantOptions = tenantOptions,
                AccountGroupOptions = groupOptions
            };
        }

        public async Task<UserAccountEditViewModel?> GetEditViewModelAsync(int accountSN)
        {
            var account = await _repo.GetAccountAsync(accountSN);
            if (account == null)
                return null;

            var isDefault = string.Equals(
                account.AccountName,
                "Admin",
                StringComparison.OrdinalIgnoreCase);

            var (isOp, _) = GetTenantContext(account.TenantSN);

            var tenantOptions = await _repo.GetTenantOptionsAsync(isOp, account.TenantSN);

            //找出對應的那一筆養殖戶名稱
            var currentTenant = tenantOptions.FirstOrDefault(t => t.SN == account.TenantSN);
            var tenantName = currentTenant?.TenantName ?? string.Empty;

            // 帳號群組只看本身養殖戶的群組
            var groupOptions = await _repo.GetAccountGroupOptionsAsync(account.TenantSN);

            return new UserAccountEditViewModel
            {
                AccountSN = account.AccountSN,
                AccountName = account.AccountName,
                RealName = account.RealName,
                Phone = account.Phone,
                Email = account.Email,
                Remark = account.Remark,
                TenantSN = account.TenantSN,
                TenantName = tenantName,
                AccGroupSN = account.AccGroupSN,
                IsEnable = account.IsEnable,
                IsDefaultAccount = isDefault,
                IsOperationCompany = isOp,
                TenantOptions = tenantOptions,
                AccountGroupOptions = groupOptions
            };
        }

        /// <summary>
        /// 表單驗證失敗後，重新補齊下拉選單等資料
        /// </summary>
        public async Task<UserAccountEditViewModel> RebuildEditViewModelAsync(UserAccountEditViewModel posted)
        {
            var (isOp, _) = GetTenantContext(posted.TenantSN);

            var tenantOptions = await _repo.GetTenantOptionsAsync(isOp, posted.TenantSN);
            var groupOptions = await _repo.GetAccountGroupOptionsAsync(posted.TenantSN);
            var tenantName = tenantOptions.FirstOrDefault(t => t.SN == posted.TenantSN)?.TenantName ?? string.Empty;

            posted.IsOperationCompany = isOp;
            posted.TenantOptions = tenantOptions;
            posted.AccountGroupOptions = groupOptions;
            posted.TenantName = tenantName;

            return posted;
        }

        public async Task CreateAsync(UserAccountEditViewModel model)
        {
            // 1. 基本檢查
            if (string.IsNullOrWhiteSpace(model.AccountName))
                throw new InvalidOperationException("帳號為必填欄位。");

            if (string.IsNullOrWhiteSpace(model.Password))
                throw new InvalidOperationException("密碼為必填欄位。");

            if (model.Password != model.ConfirmPassword)
                throw new InvalidOperationException("密碼與確認密碼不一致。");

            // 2. 帳號重複檢查（同養殖戶內）
            var isDup = await _repo.IsAccountNameDuplicateAsync(model.TenantSN, model.AccountName, null);
            if (isDup)
                throw new InvalidOperationException("帳號已存在，請改用其他帳號。");

            // 3. 建立帳號
            var now = DateTime.Now;
            var currentUser = string.IsNullOrWhiteSpace(_current.Name) ? "system" : _current.Name;

            var entity = new Account
            {
                AccountName = model.AccountName,
                RealName = model.RealName,
                Password = PasswordHelper.Sha256(model.Password), 
                TenantSN = model.TenantSN,
                AccGroupSN = model.AccGroupSN,
                Phone = model.Phone,
                Email = model.Email,
                Remark = model.Remark,
                IsEnable = model.IsEnable,
                IsDeleted = false,
                CreateTime = now,
                CreateUser = currentUser
            };

            await _repo.AddAccountAsync(entity);
        }

        public async Task UpdateAsync(UserAccountEditViewModel model)
        {
            var account = await _repo.GetAccountAsync(model.AccountSN!.Value)
                          ?? throw new InvalidOperationException("找不到指定的帳號。");

            var isDefault = string.Equals(account.AccountName, "Admin", StringComparison.OrdinalIgnoreCase);

            // 預設帳號不可改為停用 / 不可改帳號名稱
            if (isDefault)
            {
                model.AccountName = account.AccountName; // 防止有人改 form
            }

            // 帳號重複檢查（同養殖戶）
            var isDup = await _repo.IsAccountNameDuplicateAsync(
                account.TenantSN,
                model.AccountName,
                account.AccountSN);

            if (isDup)
                throw new InvalidOperationException("帳號已存在，請改用其他帳號。");

            // 更新欄位
            account.AccountName = model.AccountName;
            account.RealName = model.RealName;
            account.Phone = model.Phone;
            account.Email = model.Email;
            account.Remark = model.Remark;
            account.AccGroupSN = model.AccGroupSN;

            if (!isDefault)
            {
                account.IsEnable = model.IsEnable;
            }

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if (model.Password != model.ConfirmPassword)
                    throw new InvalidOperationException("密碼與確認密碼不一致。");

                account.Password = model.Password; // TODO: 改為加密
            }

            account.ModifyTime = DateTime.Now;
            account.ModifyUser = string.IsNullOrWhiteSpace(_current.Name) ? "system" : _current.Name;

            await _repo.UpdateAccountAsync(account);
        }

        public async Task ToggleEnableAsync(int accountSN)
        {
            var account = await _repo.GetAccountAsync(accountSN)
                          ?? throw new InvalidOperationException("找不到指定的帳號。");

            var isDefault = string.Equals(account.AccountName, "Admin", StringComparison.OrdinalIgnoreCase);
            if (isDefault)
                throw new InvalidOperationException("預設帳號不可停用。");

            account.IsEnable = !account.IsEnable;
            account.ModifyTime = DateTime.Now;
            account.ModifyUser = string.IsNullOrWhiteSpace(_current.Name) ? "system" : _current.Name;

            await _repo.UpdateAccountAsync(account);
        }

        public async Task SoftDeleteAsync(int accountSN)
        {
            var account = await _repo.GetAccountAsync(accountSN)
                          ?? throw new InvalidOperationException("找不到指定的帳號。");

            var isDefault = string.Equals(account.AccountName, "Admin", StringComparison.OrdinalIgnoreCase);
            if (isDefault)
                throw new InvalidOperationException("預設帳號不可刪除。");

            account.IsDeleted = true;
            account.DeleteTime = DateTime.Now;
            account.DeleteUser = string.IsNullOrWhiteSpace(_current.Name) ? "system" : _current.Name;

            await _repo.UpdateAccountAsync(account);
        }

        public Task<List<AccountGroupOption>> GetAccountGroupsForTenantAsync(int tenantSN)
        => _repo.GetAccountGroupOptionsAsync(tenantSN);
    }
}
