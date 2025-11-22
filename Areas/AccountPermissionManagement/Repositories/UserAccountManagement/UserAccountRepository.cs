using ADAMS.Areas.AccountPermissionManagement.ViewModels.UserAccountManagement;
using ADAMS.Areas.Models;
using ADAMS.Data;
using ADAMS.Models;
using Microsoft.EntityFrameworkCore;

namespace ADAMS.Areas.AccountPermissionManagement.Repositories.UserAccountManagement
{
    public class UserAccountRepository : IUserAccountRepository
    {
        private readonly AppDbContext _context;

        public UserAccountRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 使用者帳號分頁查詢
        /// </summary>
        public async Task<(List<Account> Data, int TotalCount)> GetPagedListAsync(
            int tenantSN,
            string? keyword,
            int status,
            int page,
            int pageSize)
        {
            var query = _context.Account
                .AsNoTracking()
                .Include(a => a.AccGroup)
                .Where(a => !a.IsDeleted); // 軟刪除排除

            // (A) 養殖戶篩選（營運公司可切換養殖戶，養殖戶只看自己）
            query = query.Where(a => a.TenantSN == tenantSN);

            // (B) 關鍵字（帳號、姓名、電話、Email）
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(a =>
                    EF.Functions.Like(a.AccountName, $"%{keyword}%") ||
                    EF.Functions.Like(a.RealName ?? string.Empty, $"%{keyword}%") ||
                    EF.Functions.Like(a.Phone ?? string.Empty, $"%{keyword}%") ||
                    EF.Functions.Like(a.Email ?? string.Empty, $"%{keyword}%"));
            }

            // 啟用狀態
            switch (status)
            {
                case 1: // 啟用
                    query = query.Where(a => a.IsEnable);
                    break;
                case 2: // 停用
                    query = query.Where(a => !a.IsEnable);
                    break;
            }

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderBy(a => a.AccountSN)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalCount);
        }

        /// <summary>
        /// 養殖戶下拉選單
        /// </summary>
        public async Task<List<TenantOption>> GetTenantOptionsAsync(
            bool isOperationCompany,
            int currentTenantSN)
        {
            if (isOperationCompany)
            {
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

        /// <summary>
        /// 指定養殖戶的帳號群組下拉選單
        /// </summary>
        public async Task<List<AccountGroupOption>> GetAccountGroupOptionsAsync(int tenantSN)
        {
            return await _context.AccountGroup
                .AsNoTracking()
                .Where(g => !g.IsDeleted && g.TenantSN == tenantSN)
                .OrderBy(g => g.Name)
                .Select(g => new AccountGroupOption
                {
                    AccountGroupSN = g.AccountGroupSN,
                    Name = g.Name
                })
                .ToListAsync();
        }

        public async Task<Account?> GetAccountAsync(int accountSN)
        {
            return await _context.Account
                .Include(a => a.AccGroup)
                .FirstOrDefaultAsync(a => a.AccountSN == accountSN && !a.IsDeleted);
        }

        public async Task<bool> IsAccountNameDuplicateAsync(int tenantSN, string accountName, int? excludeAccountSN = null)
        {
            var query = _context.Account
                .Where(a => !a.IsDeleted &&
                            a.TenantSN == tenantSN &&
                            a.AccountName == accountName);

            if (excludeAccountSN.HasValue)
            {
                query = query.Where(a => a.AccountSN != excludeAccountSN.Value);
            }

            return await query.AnyAsync();
        }

        public async Task AddAccountAsync(Account account)
        {
            _context.Account.Add(account);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAccountAsync(Account account)
        {
            _context.Account.Update(account);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
