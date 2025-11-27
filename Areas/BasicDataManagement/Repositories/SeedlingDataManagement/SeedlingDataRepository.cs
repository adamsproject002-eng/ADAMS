using ADAMS.Areas.BasicDataManagement.ViewModels.SeedlingDataManagement;
using ADAMS.Areas.Models;
using ADAMS.Data;
using ADAMS.Models;
using Microsoft.EntityFrameworkCore;

namespace ADAMS.Areas.BasicDataManagement.Repositories.SeedlingDataManagement
{
    public class SeedlingDataRepository : ISeedlingDataRepository
    {
        private readonly AppDbContext _context;

        public SeedlingDataRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Fry> Data, int TotalCount)> GetPagedListAsync(
            int tenantSN,
            int selectedFVSN,
            string? keyword,
            int page,
            int pageSize)
        {
            var query = _context.Fry
                .Include(f => f.Supplier)
                .Include(f => f.FishVariety)
                .Where(f => !f.IsDeleted && f.TenantSN == tenantSN);

            if (selectedFVSN > 0)
            {
                query = query.Where(f => f.FVSN == selectedFVSN);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(f =>
                    f.FryName.Contains(keyword) ||
                    (f.Supplier != null && f.Supplier.SupplierName.Contains(keyword)) ||
                    (f.UnitName != null && f.UnitName.Contains(keyword)));
            }

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderBy(f => f.FryName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalCount);
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

        public async Task<List<FishVarietyOption>> GetFishVarietyOptionsAsync()
        {
            return await _context.FishVariety
                .Where(v => !v.IsDeleted)
                .OrderBy(v => v.FVName)
                .Select(v => new FishVarietyOption
                {
                    FVSN = v.FVSN,
                    FVName = v.FVName
                })
                .ToListAsync();
        }

        public async Task<List<SupplierOption>> GetSupplierOptionsAsync(int tenantSN)
        {
            return await _context.Supplier
                .Where(s => !s.IsDeleted && s.TenantSN == tenantSN && s.SupplierType == 2) //2:魚苗供應商
                .OrderBy(s => s.SupplierName)
                .Select(s => new SupplierOption
                {
                    SupplierSN = s.SupplierSN,
                    SupplierName = s.SupplierName
                })
                .ToListAsync();
        }

        public Task<Fry?> GetAsync(int frySN)
        {
            return _context.Fry
                .Include(f => f.Supplier)
                .Include(f => f.FishVariety)
                .FirstOrDefaultAsync(f => f.FrySN == frySN && !f.IsDeleted);
        }

        public async Task AddAsync(Fry entity)
        {
            _context.Fry.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Fry entity)
        {
            _context.Fry.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int frySN, string currentUser)
        {
            var entity = await _context.Fry.FirstOrDefaultAsync(f => f.FrySN == frySN);
            if (entity == null) return;

            entity.IsDeleted = true;
            entity.DeleteTime = DateTime.Now;
            entity.DeleteUser = currentUser;

            await _context.SaveChangesAsync();
        }
    }
}
