using ADAMS.Areas.Models;
using ADAMS.Data;
using Microsoft.EntityFrameworkCore;

namespace ADAMS.Areas.BasicDataManagement.Repositories.TimeSegmentSetting
{
    public class TimeSegmentRepository : ITimeSegmentRepository
    {
        private readonly AppDbContext _context;

        public TimeSegmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(List<ADAMS.Models.TimeZone> Data, int TotalCount)> GetPagedListAsync(
            int tenantSN,
            string? keyword,
            int page,
            int pageSize)
        {
            var query = _context.TimeZone
                .AsNoTracking()
                .Where(t => !t.IsDeleted && t.TenantSN == tenantSN);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(t =>
                    t.TimeZoneNum.Contains(keyword) ||
                    t.TimeZoneDesc.Contains(keyword));
            }

            var total = await query.CountAsync();

            var data = await query
                .OrderBy(t => t.TimeZoneNum)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, total);
        }

        public async Task<List<TenantOption>> GetTenantOptionsAsync(
            bool isOperationCompany,
            int currentTenantSN)
        {
            if (isOperationCompany)
            {
                return await _context.Tenant
                    .Where(t => t.IsEnable && t.SN != 0)
                    .OrderBy(t => t.CreateTime)
                    .Select(t => new TenantOption
                    {
                        SN = t.SN,
                        TenantName = t.TenantName
                    })
                    .ToListAsync();
            }

            return await _context.Tenant
                .Where(t => t.IsEnable && t.SN == currentTenantSN)
                .Select(t => new TenantOption
                {
                    SN = t.SN,
                    TenantName = t.TenantName
                })
                .ToListAsync();
        }

        public Task<ADAMS.Models.TimeZone?> GetByIdAsync(int timeZoneSN)
        {
            return _context.TimeZone
                .FirstOrDefaultAsync(t => t.TimeZoneSN == timeZoneSN && !t.IsDeleted);
        }

        public async Task<bool> CheckTimeZoneNumExistsAsync(string timeZoneNum, int tenantSN, int? excludeTimeZoneSN = null)
        {
            var query = _context.TimeZone
                .Where(t => !t.IsDeleted &&
                           t.TenantSN == tenantSN &&
                           t.TimeZoneNum == timeZoneNum);

            if (excludeTimeZoneSN.HasValue)
            {
                query = query.Where(t => t.TimeZoneSN != excludeTimeZoneSN.Value);
            }

            return await query.AnyAsync();
        }

        public async Task AddAsync(ADAMS.Models.TimeZone timeZone)
        {
            _context.TimeZone.Add(timeZone);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ADAMS.Models.TimeZone timeZone)
        {
            _context.TimeZone.Update(timeZone);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int timeZoneSN, string currentUser)
        {
            var entity = await _context.TimeZone.FirstOrDefaultAsync(t => t.TimeZoneSN == timeZoneSN);
            if (entity == null) return;

            entity.IsDeleted = true;
            entity.DeleteTime = DateTime.Now;
            entity.DeleteUser = currentUser;

            await _context.SaveChangesAsync();
        }
    }
}

