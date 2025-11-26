using ADAMS.Areas.Models;
using ADAMS.Data;
using ADAMS.Models;
using Microsoft.EntityFrameworkCore;

namespace ADAMS.Areas.BasicDataManagement.Repositories.AreaPondManagement
{
    public class AreaPondRepository : IAreaPondRepository
    {
        private readonly AppDbContext _context;

        public AreaPondRepository(AppDbContext context)
        {
            _context = context;
        }

        // ---------- Area ----------

        public async Task<(List<Area> Data, int TotalCount)> GetAreaPagedListAsync(
            int tenantSN,
            string? keyword,
            int page,
            int pageSize)
        {
            var query = _context.Area
                .Include(a => a.Ponds)
                .Where(a => a.TenantSN == tenantSN && !a.IsDeleted);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(a =>
                    a.AreaName.Contains(keyword) ||
                    (a.GPSX ?? "").Contains(keyword) ||
                    (a.GPSY ?? "").Contains(keyword));
            }

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderBy(a => a.AreaName)
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

        public async Task<Area?> GetAreaAsync(int areaSN)
        {
            return await _context.Area
                .Include(a => a.Ponds)
                .FirstOrDefaultAsync(a => a.AreaSN == areaSN && !a.IsDeleted);
        }

        public async Task AddAreaAsync(Area area)
        {
            _context.Area.Add(area);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAreaAsync(Area area)
        {
            _context.Area.Update(area);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasPondsAsync(int areaSN)
        {
            return await _context.Pond
                .AnyAsync(p => p.AreaSN == areaSN && !p.IsDeleted);
        }

        public async Task SoftDeleteAreaAsync(int areaSN, string currentUser)
        {
            var entity = await _context.Area.FirstOrDefaultAsync(a => a.AreaSN == areaSN);
            if (entity == null) return;

            entity.IsDeleted = true;
            entity.DeleteTime = DateTime.Now;
            entity.DeleteUser = currentUser;

            await _context.SaveChangesAsync();
        }

        // ---------- Pond ----------

        public async Task<(List<Pond> Data, int TotalCount, decimal TotalArea)> GetPondsPagedListAsync(
            int areaSN,
            string? keyword,
            int page,
            int pageSize)
        {
            var query = _context.Pond
                .Where(p => p.AreaSN == areaSN && !p.IsDeleted);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(p => p.PondNum.Contains(keyword));
            }

            var totalCount = await query.CountAsync();
            var totalArea = await query.SumAsync(p => p.PondArea);

            var data = await query
                .OrderBy(p => p.PondNum)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalCount, totalArea);
        }

        public async Task<Pond?> GetPondAsync(int pondSN)
        {
            return await _context.Pond
                .FirstOrDefaultAsync(p => p.PondSN == pondSN && !p.IsDeleted);
        }

        public async Task AddPondAsync(Pond pond)
        {
            _context.Pond.Add(pond);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePondAsync(Pond pond)
        {
            _context.Pond.Update(pond);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeletePondAsync(int pondSN, string currentUser)
        {
            var entity = await _context.Pond.FirstOrDefaultAsync(p => p.PondSN == pondSN);
            if (entity == null) return;

            entity.IsDeleted = true;
            entity.DeleteTime = DateTime.Now;
            entity.DeleteUser = currentUser;

            await _context.SaveChangesAsync();
        }
    }
}
