using ADAMS.Areas.OperationManagement.Repositories.FryRecords;
using ADAMS.Data;
using ADAMS.Models;
using Microsoft.EntityFrameworkCore;

namespace ADAMS.Areas.OperationManagement.Repositories.FryRecords
{
    public class FryRecordRepositoryService : IFryRecordRepository
    {
        private readonly AppDbContext _context;

        public FryRecordRepositoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(List<FryRecord> Data, int TotalCount)> GetPagedListAsync(
            int tenantSN,
            int? areaSN = null,
            int? pondSN = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int page = 1,
            int pageSize = 20)
        {
            var query = _context.FryRecord
                .Include(f => f.Pond)
                .ThenInclude(p => p.Area)
                .Where(f => !f.IsDeleted && f.Pond.TenantSN == tenantSN)
                .AsNoTracking();

            if (areaSN.HasValue)
                query = query.Where(f => f.Pond.AreaSN == areaSN.Value);

            if (pondSN.HasValue)
                query = query.Where(f => f.PondSN == pondSN.Value);

            if (startDate.HasValue)
                query = query.Where(f => f.FarmingDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(f => f.FarmingDate <= endDate.Value);

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderByDescending(f => f.FarmingDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalCount);
        }

        public async Task<FryRecord?> GetByIdAsync(int id)
        {
            return await _context.FryRecord
                .Include(f => f.Pond)
                .ThenInclude(p => p.Area)
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.FryRecordSN == id && !f.IsDeleted);
        }

        public async Task<FryRecord> CreateAsync(FryRecord fryRecord)
        {
            _context.FryRecord.Add(fryRecord);
            await _context.SaveChangesAsync();
            return fryRecord;
        }

        public async Task<bool> UpdateAsync(FryRecord fryRecord)
        {
            _context.FryRecord.Update(fryRecord);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SoftDeleteAsync(int id, string deleteUser)
        {
            var record = await _context.FryRecord.FindAsync(id);
            if (record == null) return false;

            record.IsDeleted = true;
            record.DeleteTime = DateTime.Now;
            record.DeleteUser = deleteUser;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<FryRecord>> GetAllActiveAsync(int tenantSN)
        {
            return await _context.FryRecord
                .Include(f => f.Pond)
                .ThenInclude(p => p.Area)
                .Where(f => !f.IsDeleted && f.Pond.TenantSN == tenantSN)
                .OrderByDescending(f => f.FarmingDate)
                .ToListAsync();
        }
    }
}
