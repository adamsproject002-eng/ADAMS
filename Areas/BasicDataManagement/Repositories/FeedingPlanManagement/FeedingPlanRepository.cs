using ADAMS.Areas.Models;
using ADAMS.Data;
using ADAMS.Models;
using Microsoft.EntityFrameworkCore;

namespace ADAMS.Areas.BasicDataManagement.Repositories.FeedingPlanManagement
{
    public class FeedingPlanRepository : IFeedingPlanRepository
    {
        private readonly AppDbContext _context;

        public FeedingPlanRepository(AppDbContext context)
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

        public async Task<List<FishVariety>> GetFishVarietiesAsync()
        {
            return await _context.FishVariety
                .Where(v => !v.IsDeleted)
                .OrderBy(v => v.FVName)
                .ToListAsync();
        }

        public async Task<FeedingPlanMain?> GetMainAsync(int tenantSN, int fvsn)
        {
            return await _context.FeedingPlanMain
                .Include(m => m.FishVariety)
                .FirstOrDefaultAsync(m =>
                    !m.IsDeleted &&
                    m.TenantSN == tenantSN &&
                    m.FVSN == fvsn);
        }

        public async Task<FeedingPlanMain> CreateMainAsync(int tenantSN, int fvsn, string planName, string currentUser)
        {
            var entity = new FeedingPlanMain
            {
                TenantSN = tenantSN,
                FVSN = fvsn,
                FeedingPlanName = planName,
                IsDeleted = false,
                CreateTime = DateTime.Now,
                CreateUser = currentUser
            };

            _context.FeedingPlanMain.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<List<FeedingPlanDetail>> GetDetailsAsync(int fpmsn)
        {
            return await _context.FeedingPlanDetail
                .Where(d => d.FPMSN == fpmsn && !d.IsDeleted)
                .OrderBy(d => d.FP_DOC)
                .ToListAsync();
        }

        public async Task<FeedingPlanDetail?> GetDetailAsync(int fpdsn)
        {
            return await _context.FeedingPlanDetail
                .Include(d => d.Main).ThenInclude(m => m.FishVariety)
                .Include(d => d.Main).ThenInclude(m => m.Tenant)
                .FirstOrDefaultAsync(d => d.FPDSN == fpdsn && !d.IsDeleted);
        }

        public async Task AddDetailAsync(FeedingPlanDetail detail)
        {
            _context.FeedingPlanDetail.Add(detail);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDetailAsync(FeedingPlanDetail detail)
        {
            _context.FeedingPlanDetail.Update(detail);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteDetailAsync(int fpdsn, string currentUser)
        {
            var entity = await _context.FeedingPlanDetail
                .FirstOrDefaultAsync(d => d.FPDSN == fpdsn);
            if (entity == null) return;

            entity.IsDeleted = true;
            entity.DeleteTime = DateTime.Now;
            entity.DeleteUser = currentUser;

            await _context.SaveChangesAsync();
        }
    }
}
