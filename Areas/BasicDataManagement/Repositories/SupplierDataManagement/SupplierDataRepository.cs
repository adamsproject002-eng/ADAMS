using ADAMS.Areas.BasicDataManagement.ViewModels.SupplierDataManagement;
using ADAMS.Areas.Models;
using ADAMS.Data;
using ADAMS.Models;
using Microsoft.EntityFrameworkCore;

namespace ADAMS.Areas.BasicDataManagement.Repositories.SupplierDataManagement
{
    public class SupplierDataRepository : ISupplierDataRepository
    {
        private readonly AppDbContext _context;

        public SupplierDataRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Supplier> Data, int TotalCount)> GetPagedListAsync(
            int tenantSN,
            string? keyword,
            int supplierType,
            int page,
            int pageSize)
        {
            var query = _context.Supplier
                .AsNoTracking()
                .Where(s => !s.IsDeleted && s.TenantSN == tenantSN);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(s =>
                    s.SupplierNum.Contains(keyword) ||
                    s.SupplierName.Contains(keyword) ||
                    (s.ContactName != null && s.ContactName.Contains(keyword)) ||
                    (s.ContactPhone != null && s.ContactPhone.Contains(keyword)));
            }

            if (supplierType > 0)
            {
                query = query.Where(s => s.SupplierType == supplierType);
            }

            var total = await query.CountAsync();

            var data = await query
                .OrderBy(s => s.SupplierNum)
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

        public Task<Supplier?> GetByIdAsync(int supplierSN)
        {
            return _context.Supplier
                .FirstOrDefaultAsync(s => s.SupplierSN == supplierSN && !s.IsDeleted);
        }

        public async Task AddAsync(Supplier supplier)
        {
            _context.Supplier.Add(supplier);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Supplier supplier)
        {
            _context.Supplier.Update(supplier);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int supplierSN, string currentUser)
        {
            var entity = await _context.Supplier.FirstOrDefaultAsync(s => s.SupplierSN == supplierSN);
            if (entity == null) return;

            entity.IsDeleted = true;
            entity.DeleteTime = DateTime.Now;
            entity.DeleteUser = currentUser;

            await _context.SaveChangesAsync();
        }
    }
}
