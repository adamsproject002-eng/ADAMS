using ADAMS.Areas.Models;
using ADAMS.Areas.PondOverview.ViewModels.PondOverviewPage;
using ADAMS.Data;
using ADAMS.Models;
using Microsoft.EntityFrameworkCore;

namespace ADAMS.Areas.PondOverview.Repositories.PondOverviewPage
{
    public class PondOverviewRepository : IPondOverviewRepository
    {
        private readonly AppDbContext _context;

        public PondOverviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TenantOption>> GetTenantOptionsAsync(
             bool isOperationCompany,
             int currentTenantSN)
        {
            if (isOperationCompany)
            {
                // 營運帳號可以看所有養殖戶（排除 SN = 0）
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
                //非營運帳號只允許使用自己的 Tenant
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

        public async Task<List<AreaOption>> GetAreaOptionsAsync(int tenantSN)
        {
            return await _context.Area
                .Where(a => a.TenantSN == tenantSN && !a.IsDeleted)
                .OrderBy(a => a.AreaName)
                .Select(a => new AreaOption
                {
                    AreaSN = a.AreaSN,
                    AreaName = a.AreaName
                })
                .ToListAsync();
        }

        public async Task<List<PondCardViewModel>> GetPondCardsAsync(int tenantSN, int? areaSN)
        {
            var query = _context.Pond
                .Where(p => p.TenantSN == tenantSN && !p.IsDeleted);

            if (areaSN.HasValue && areaSN.Value > 0)
            {
                query = query.Where(p => p.AreaSN == areaSN.Value);
            }

            // ★ 目前只能確定 PondArea / PondNum 等欄位
            //   放養尾數 / 品種 / ABW 之後可以從實際營運資料表 join 回來
            return await query
                .OrderBy(p => p.PondNum)
                .Select(p => new PondCardViewModel
                {
                    PondSN = p.PondSN,
                    PondNum = p.PondNum,
                    PondArea = p.PondArea,
                    StockingQty = 0,             // TODO: 之後改為實際放養尾數
                    FishVarietyName = "",        // TODO: 之後改為實際魚種名稱
                    CurrentABW = 0m              // TODO: 之後改為實際 ABW
                })
                .ToListAsync();
        }

        public async Task<PondDetailViewModel?> GetPondDetailAsync(int pondSN)
        {
            var pond = await _context.Pond
                .Include(p => p.Area)
                .Include(p => p.Tenant)
                .FirstOrDefaultAsync(p => p.PondSN == pondSN && !p.IsDeleted);

            if (pond == null) return null;

            // ★ 下面多數欄位先留預留值，等你接實際表格（放養紀錄 / 採樣紀錄 / 收成紀錄等）後再改查詢
            return new PondDetailViewModel
            {
                PondSN = pond.PondSN,
                PondNum = pond.PondNum,
                PondArea = pond.PondArea,
                AreaName = pond.Area?.AreaName ?? "",
                TenantName = pond.Tenant?.TenantName ?? "",

                ManagerName = "",                 // TODO: 管理員
                StockingCode = "",                // TODO: 放養碼
                FishVarietyName = "",             // TODO: 放養種類
                PLorGramUnit = "",                // TODO: PL 或 克/尾
                StockingQty = 0,                  // TODO
                Density = 0m,                     // TODO

                StockingDate = null,              // TODO
                LastSampleOrHarvestDate = null,   // TODO
                CurrentABW = 0m,                  // TODO
                DOC = 0,                          // TODO

                AccumulatedHarvestWeight = 0m,    // TODO
                HarvestTimes = 0,                 // TODO
                SurvivalRate = 0m,                // TODO

                Remark = pond.Remark
            };
        }
    }
}
