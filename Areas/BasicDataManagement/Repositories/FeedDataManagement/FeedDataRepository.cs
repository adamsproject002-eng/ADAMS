using ADAMS.Areas.BasicDataManagement.Repositories.FeedDataManagement;
using ADAMS.Areas.Models;
using ADAMS.Data;
using ADAMS.Models;
using Microsoft.EntityFrameworkCore;

namespace ADAMS.Areas.BasicDataManagement.Repositories.FeedDataManagement
{
    public class FeedDataRepository : IFeedDataRepository
    {
        private readonly AppDbContext _context;

        public FeedDataRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TenantOption>> GetTenantOptionsAsync(bool isOperationCompany, int currentTenantSN)
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

        public async Task<List<Supplier>> GetSuppliersForTenantAsync(int tenantSN)
        {
            // 只抓該養殖戶、未刪除的供應商
            return await _context.Supplier
                .Where(s => s.TenantSN == tenantSN && !s.IsDeleted && s.SupplierType == 1) // 1:飼料供應商
                .OrderBy(s => s.SupplierName)
                .ToListAsync();
        }

        public async Task<(List<Feed> Data, int TotalCount)> GetPagedListAsync(
            int tenantSN,
            string? keyword,
            int page,
            int pageSize)
        {
            var query = _context.Feed
                .Include(f => f.Supplier)
                .Where(f => f.TenantSN == tenantSN && !f.IsDeleted);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(f =>
                    f.FeedName.Contains(keyword) ||
                    (f.Supplier != null && f.Supplier.SupplierName.Contains(keyword)));
            }

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderBy(f => f.FeedSN)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalCount);
        }

        public async Task<Feed?> GetAsync(int feedSN)
        {
            return await _context.Feed
                .Include(f => f.Tenant)
                .Include(f => f.Supplier)
                .FirstOrDefaultAsync(f => f.FeedSN == feedSN);
        }

        public async Task CreateAsync(Feed feed)
        {
            _context.Feed.Add(feed);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Feed feed)
        {
            _context.Feed.Update(feed);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int feedSN, string currentUser)
        {
            var entity = await _context.Feed.FirstOrDefaultAsync(f => f.FeedSN == feedSN);
            if (entity == null) return;

            entity.IsDeleted = true;
            entity.DeleteTime = DateTime.Now;
            entity.DeleteUser = currentUser;

            await _context.SaveChangesAsync();
        }
    }
}
