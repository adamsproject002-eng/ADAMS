using Microsoft.EntityFrameworkCore;
using ADAMS.Data;
using ADAMS.Models;

namespace ADAMS.Areas.AccountPermissionManagement.Repositories.FarmerManagement
{
    public class FarmerRepository : IFarmerRepository
    {
        private readonly AppDbContext _context;

        public FarmerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Tenant> Data, int TotalCount)> GetPagedListAsync(
            string? statusFilter,
            string? keyword,
            int page,
            int pageSize)
        {
            var query = _context.Tenant.AsNoTracking();

            // 狀態篩選
            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter == "啟用")
                {
                    query = query.Where(t => t.IsEnable);
                }
                else if (statusFilter == "停用")
                {
                    query = query.Where(t => !t.IsEnable);
                }
            }
            //關鍵字篩選
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x =>
                    EF.Functions.Like(x.TenantNum, $"%{keyword}%") ||
                    EF.Functions.Like(x.TenantName, $"%{keyword}%") ||
                    EF.Functions.Like(x.ResponName ?? "", $"%{keyword}%") ||
                    EF.Functions.Like(x.ResponPhone ?? "", $"%{keyword}%") ||
                    EF.Functions.Like(x.ResponEmail ?? "", $"%{keyword}%")
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

        public async Task<Tenant?> GetByIdAsync(int id, bool includeRelations = false)
        {
            var query = _context.Tenant.AsNoTracking().Where(t => t.SN == id);

            if (includeRelations)
            {
                query = query
                    .Include(t => t.Accounts.Where(a => a.IsEnable))
                        .ThenInclude(a => a.AccGroup)
                    //.Include(t => t.AccountGroups.Where(g => g.IsEnable));
                    .Include(t => t.AccountGroups);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<Tenant?> GetByNumAsync(string tenantNum)
        {
            return await _context.Tenant
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TenantNum == tenantNum);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Tenant.AnyAsync(t => t.SN == id);
        }

        public async Task<bool> NumExistsAsync(string tenantNum, int? excludeId = null)
        {
            var query = _context.Tenant.Where(t => t.TenantNum == tenantNum);

            if (excludeId.HasValue)
            {
                query = query.Where(t => t.SN != excludeId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<int> GetAccountCountAsync(int tenantId)
        {
            return await _context.Account
                .CountAsync(a => a.TenantSN == tenantId && a.IsEnable);
        }

        public async Task<Tenant> CreateAsync(Tenant tenant)
        {
            _context.Tenant.Add(tenant);
            await _context.SaveChangesAsync();
            return tenant;
        }

        public async Task<bool> UpdateAsync(Tenant tenant)
        {
            _context.Tenant.Update(tenant);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ToggleStatusAsync(int id, bool isEnable)
        {
            var tenant = await _context.Tenant.FindAsync(id);
            if (tenant == null) return false;

            tenant.IsEnable = isEnable;
            tenant.ModifyTime = DateTime.Now;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Tenant>> GetAllActiveAsync()
        {
            return await _context.Tenant
                .AsNoTracking()
                .Where(t => t.IsEnable)
                .OrderBy(t => t.TenantName)
                .ToListAsync();
        }
    }
}