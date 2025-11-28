using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ADAMS.Data;
using ADAMS.Models;
using Microsoft.EntityFrameworkCore;

namespace ADAMS.Areas.BasicDataManagement.Repositories.UnitDataManagement
{
    public class UnitDataRepository : IUnitDataRepository
    {
        private readonly AppDbContext _context;

        public UnitDataRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Unit>> GetListAsync(string? keyword)
        {
            var query = _context.Unit
                .Where(u => !u.IsDeleted);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(u =>
                    u.UnitName.Contains(keyword) ||
                    (u.Remark != null && u.Remark.Contains(keyword)));
            }

            return await query
                .OrderBy(u => u.UnitName)
                .ToListAsync();
        }

        public async Task<Unit?> GetByIdAsync(int unitSn)
        {
            return await _context.Unit
                .FirstOrDefaultAsync(u => u.UnitSN == unitSn && !u.IsDeleted);
        }

        public async Task<bool> ExistsByNameAsync(string unitName, int? excludeUnitSn = null)
        {
            var query = _context.Unit
                .Where(u => u.UnitName == unitName && !u.IsDeleted);

            if (excludeUnitSn.HasValue)
            {
                query = query.Where(u => u.UnitSN != excludeUnitSn.Value);
            }

            return await query.AnyAsync();
        }

        public async Task AddAsync(Unit entity)
        {
            _context.Unit.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Unit entity)
        {
            _context.Unit.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int unitSn, string currentUser)
        {
            var entity = await _context.Unit
                .FirstOrDefaultAsync(u => u.UnitSN == unitSn && !u.IsDeleted);

            if (entity == null) return;

            entity.IsDeleted = true;
            entity.DeleteTime = DateTime.Now;
            entity.DeleteUser = currentUser;

            await _context.SaveChangesAsync();
        }
    }
}
