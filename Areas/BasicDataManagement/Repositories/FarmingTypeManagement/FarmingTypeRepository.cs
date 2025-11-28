using ADAMS.Data;
using ADAMS.Models;
using Microsoft.EntityFrameworkCore;

namespace ADAMS.Areas.BasicDataManagement.Repositories.FarmingTypeManagement
{
    public class FarmingTypeRepository : IFarmingTypeRepository
    {
        private readonly AppDbContext _context;

        public FarmingTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<FishVariety>> GetListAsync(string? keyword)
        {
            var query = _context.FishVariety
                .Where(v => !v.IsDeleted);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(v =>
                    v.FVName.Contains(keyword) ||
                    (v.Remark != null && v.Remark.Contains(keyword)));
            }

            return await query
                .OrderBy(v => v.FVName)
                .ToListAsync();
        }

        public async Task<FishVariety?> GetByIdAsync(int fvsn)
        {
            return await _context.FishVariety
                .FirstOrDefaultAsync(v => v.FVSN == fvsn && !v.IsDeleted);
        }

        public async Task<bool> ExistsByNameAsync(string fvName, int? excludeFvsn = null)
        {
            var query = _context.FishVariety
                .Where(v => v.FVName == fvName && !v.IsDeleted);

            if (excludeFvsn.HasValue)
            {
                query = query.Where(v => v.FVSN != excludeFvsn.Value);
            }

            return await query.AnyAsync();
        }

        public async Task AddAsync(FishVariety entity)
        {
            _context.FishVariety.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FishVariety entity)
        {
            _context.FishVariety.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int fvsn, string currentUser)
        {
            var entity = await _context.FishVariety
                .FirstOrDefaultAsync(v => v.FVSN == fvsn && !v.IsDeleted);

            if (entity == null) return;

            entity.IsDeleted = true;
            entity.DeleteTime = DateTime.Now;
            entity.DeleteUser = currentUser;

            await _context.SaveChangesAsync();
        }
    }
}
