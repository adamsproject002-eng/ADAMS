using ADAMS.Areas.AccountPermissionManagement.ViewModels.GroupPermissionManagement;
using ADAMS.Data;
using ADAMS.Models;
using Microsoft.EntityFrameworkCore;

namespace ADAMS.Areas.AccountPermissionManagement.Repositories.GroupPermissionManagement
{
    public class GroupPermissionRepository : IGroupPermissionRepository
    {
        private readonly AppDbContext _context;

        public GroupPermissionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(List<AccountGroup> Data, int TotalCount)> GetPagedListAsync(int tenantSN, string? groupName, int page, int pageSize)
        {
            var query = _context.AccountGroup.AsNoTracking();

            query = query.Where(g => g.TenantSN == tenantSN);

            //群組名稱篩選
            if (!string.IsNullOrEmpty(groupName))
            {
                query = query.Where(x =>
                    EF.Functions.Like(x.Name, $"%{groupName}%")
                );
            }

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderByDescending(t => t.CreateTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalCount);
        }

        public async Task<List<TenantOption>> GetTenantOptionsAsync(
             bool isOperationCompany,
             int currentTenantSN)
        {
            if (isOperationCompany)
            {
                // 營運帳號可以看所有養殖戶（排除 SN = 0）
                return await _context.Tenant
                    .Where(t => t.SN != 0 && t.IsEnable)
                    .OrderBy(t => t.CreateTime)
                    .Select(t => new TenantOption
                    {
                        SN = t.SN,
                        TenantName = t.TenantName
                    })
                    .ToListAsync();
            }
            else
            {
                //非營運帳號只允許使用自己的 Tenant
                return await _context.Tenant
                    .Where(t => t.SN == currentTenantSN)
                    .Select(t => new TenantOption
                    {
                        SN = t.SN,
                        TenantName = t.TenantName
                    })
                    .ToListAsync();
            }
        }

        public async Task<List<Account>> GetAccountsByGroupAsync(int groupSn)
        {
            return await _context.Account
                    .Where(a => a.AccGroupSN == groupSn && a.IsEnable)
                    .OrderBy(a => a.AccountSN)
                    .Select(a => new Account
                    {
                        AccountSN = a.AccountSN,
                        AccountName = a.AccountName
                    })
                    .ToListAsync();
        }

        public Task<AccountGroup?> GetGroupAsync(int accountGroupSN)
        {
            return _context.AccountGroup
                    .AsNoTracking().FirstOrDefaultAsync(g => g.AccountGroupSN == accountGroupSN);
        }

        public Task<List<Function>> GetAllMenuFunctionsAsync()
        {
            return _context.Function
                .AsNoTracking()
                .Where(f => f.IsShow)
                .Where(f => f.Name != "FarmerManagement") // 排除養殖戶管理功能(運營人員才有)
                .OrderBy(f => f.FLevel)
                .ThenBy(f => f.Sort)
                .ToListAsync();
        }

        public Task<List<int>> GetGrantedFunctionSNsAsync(int accountGroupSN)
        {
            return _context.Authorization
                .AsNoTracking()
                .Where(a => a.AccGroupSN == accountGroupSN && !a.IsDeleted)
                .Select(a => a.FunctionSN)
                .ToListAsync();
        }

        public Task<Tenant?> GetTenantAsync(int tenantSN)
            => _context.Tenant
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.SN == tenantSN);

        public async Task AddGroupAsync(AccountGroup group)
        {
            _context.AccountGroup.Add(group);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateGroupAsync(AccountGroup group)
        {
            _context.AccountGroup.Update(group);
            await _context.SaveChangesAsync();
        }

        public async Task ReplaceAuthorizationsAsync(int accountGroupSN, IEnumerable<int> functionSNs , string? currentAccount)
        {
            // 先抓舊資料
            var oldAuths = await _context.Authorization
                .Where(a => a.AccGroupSN == accountGroupSN)
                .ToListAsync();

            _context.Authorization.RemoveRange(oldAuths);

            var now = DateTime.Now;

            var newAuths = functionSNs.Distinct().Select(fn => new Authorization
            {
                AccGroupSN = accountGroupSN,
                FunctionSN = fn,
                IsDeleted = false,
                CreateTime = now,
                CreateUser = currentAccount ?? "system" ,
            });

            await _context.Authorization.AddRangeAsync(newAuths);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasAccountsAsync(int accountGroupSN)
        {
            return await _context.Account
                .AsNoTracking()
                .AnyAsync(a =>
                    a.AccGroupSN == accountGroupSN &&
                    !a.IsDeleted);            // 只看未軟刪除的帳號
        }

        public async Task SoftDeleteAuthorizationsByGroupAsync(int accountGroupSN, string currentUser)
        {
            var now = DateTime.Now;

            var auths = await _context.Authorization
                .Where(a => a.AccGroupSN == accountGroupSN && !a.IsDeleted)
                .ToListAsync();

            if (auths.Count == 0)
                return;                      // 沒有資料就不用 SaveChanges

            foreach (var a in auths)
            {
                a.IsDeleted = true;
                a.DeleteTime = now;
                a.DeleteUser = currentUser;
            }

            await _context.SaveChangesAsync();
        }
    }
}
