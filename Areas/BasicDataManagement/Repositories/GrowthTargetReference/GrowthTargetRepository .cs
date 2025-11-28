using ADAMS.Data;
using ADAMS.Models;
using Microsoft.EntityFrameworkCore;

namespace ADAMS.Areas.BasicDataManagement.Repositories.GrowthTargetReference
{
    public class GrowthTargetRepository : IGrowthTargetRepository
    {
        private readonly AppDbContext _context;

        public GrowthTargetRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<FishVariety>> GetFishVarietiesAsync()
        {
            return await _context.FishVariety
                .Where(v => !v.IsDeleted)
                .OrderBy(v => v.FVName)
                .ToListAsync();
        }

        public async Task<GrowTargetMain?> GetMainByFVSNAsync(int fvsn)
        {
            return await _context.GrowTargetMain
                .Include(m => m.FishVariety)
                .Where(m => m.FVSN == fvsn && !m.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<GrowTargetMain> CreateMainAsync(int fvsn, string gtmName, string currentUser)
        {
            var main = new GrowTargetMain
            {
                FVSN = fvsn,
                GTMName = gtmName,
                IsDeleted = false,
                CreateTime = DateTime.Now,
                CreateUser = currentUser
            };

            _context.GrowTargetMain.Add(main);
            await _context.SaveChangesAsync();
            return main;
        }

        public async Task<List<GrowTargetDetail>> GetDetailsAsync(int gtmsn)
        {
            return await _context.GrowTargetDetail
                .Where(d => d.GTMSN == gtmsn && !d.IsDeleted)
                .OrderBy(d => d.GT_DOC)
                .ToListAsync();
        }

        public async Task<GrowTargetDetail?> GetDetailAsync(int gtdsn)
        {
            return await _context.GrowTargetDetail
                .Include(d => d.Main)
                .ThenInclude(m => m.FishVariety)
                .FirstOrDefaultAsync(d => d.GTDSN == gtdsn && !d.IsDeleted);
        }

        public async Task AddDetailAsync(GrowTargetDetail detail)
        {
            _context.GrowTargetDetail.Add(detail);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDetailAsync(GrowTargetDetail detail)
        {
            _context.GrowTargetDetail.Update(detail);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteDetailAsync(int gtdsn, string currentUser)
        {
            var entity = await _context.GrowTargetDetail
                .FirstOrDefaultAsync(d => d.GTDSN == gtdsn);

            if (entity == null) return;

            entity.IsDeleted = true;
            entity.DeleteTime = DateTime.Now;
            entity.DeleteUser = currentUser;

            await _context.SaveChangesAsync();
        }
    }
}
