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

            // 找最新一筆尚未刪除的放苗紀錄
            var fry = await _context.FryRecord
                .Include(fr => fr.Fry)
                    .ThenInclude(f => f.FishVariety)
                .Where(fr => fr.PondSN == pondSN && !fr.IsDeleted)
                .OrderByDescending(fr => fr.FarmingDate)
                .FirstOrDefaultAsync();

            string managerName = "";
            string farmingCode = "";
            string fishVarietyName = "";
            string plOrGram = "";
            int stockingQty = 0;
            decimal density = 0m;
            DateTime? stockingDate = null;

            decimal currentABW = 0m;
            int doc = 0;
            DateTime? lastSampleOrHarvestDate = null;

            decimal totalHarvestWeight = 0m;
            int totalHarvestPCS = 0;
            int harvestTimes = 0;
            decimal survivalRate = 0m;

            if (fry != null)
            {
                managerName = fry.ManageAccount ?? "";
                farmingCode = fry.FarmingCode;
                fishVarietyName = fry.Fry?.FishVariety?.FVName ?? "";
                stockingQty = fry.FarmingPCS;
                stockingDate = fry.FarmingDate;

                // 密度：尾數 / 面積
                if (pond.PondArea > 0)
                    density = fry.FarmingDensity > 0
                        ? fry.FarmingDensity
                        : Math.Round((decimal)fry.FarmingPCS / pond.PondArea, 2, MidpointRounding.AwayFromZero);

                plOrGram = fry.FryAge.ToString("0.##");
            }

            if (!string.IsNullOrEmpty(farmingCode))
            {
                //找最近的採樣與收穫，取日期較新的那一筆當「最近收成/採樣日」+ ABW/DOC
                var lastSampling = await _context.SamplingRecord
                    .Where(s => s.PondSN == pondSN &&
                                s.FarmingCode == farmingCode &&
                                !s.IsDeleted)
                    .OrderByDescending(s => s.SamplingDate)
                    .FirstOrDefaultAsync();

                var lastHarvest = await _context.HarvestRecord
                    .Where(h => h.PondSN == pondSN &&
                                h.FarmingCode == farmingCode &&
                                !h.IsDeleted)
                    .OrderByDescending(h => h.HarvestDate)
                    .FirstOrDefaultAsync();

                if (lastSampling != null || lastHarvest != null)
                {
                    // 採樣比較新 or 只有採樣
                    if (lastHarvest == null ||
                        (lastSampling != null && lastSampling.SamplingDate >= lastHarvest.HarvestDate))
                    {
                        lastSampleOrHarvestDate = lastSampling!.SamplingDate;
                        currentABW = lastSampling.ABW;
                        doc = lastSampling.DOC;
                    }
                    // 收穫比較新
                    else
                    {
                        lastSampleOrHarvestDate = lastHarvest!.HarvestDate;
                        currentABW = lastHarvest.ABW;
                        doc = lastHarvest.DOC;
                    }
                }

                //累計收成重量 / 尾數 / 次數
                var harvestQuery = _context.HarvestRecord
                    .Where(h => h.PondSN == pondSN &&
                                h.FarmingCode == farmingCode &&
                                !h.IsDeleted);

                totalHarvestWeight = await harvestQuery.SumAsync(h => (decimal?)h.HarvestWeight) ?? 0m;
                totalHarvestPCS = await harvestQuery.SumAsync(h => (int?)h.HarvestPCS) ?? 0;
                harvestTimes = await harvestQuery.CountAsync();

                //存活率 = 累計收穫尾數 / 放養尾數
                if (fry != null && fry.FarmingPCS > 0)
                {
                    survivalRate = Math.Round(
                        (decimal)totalHarvestPCS / fry.FarmingPCS * 100m,
                        2,
                        MidpointRounding.AwayFromZero);
                }
            }

            // 如果沒有任何採樣/收穫紀錄，就用「今天」算目前 DOC
            if (doc == 0 && fry != null)
            {
                doc = (int)(DateTime.Today.Date - fry.FarmingDate.Date).TotalDays;
            }

            return new PondDetailViewModel
            {
                PondSN = pond.PondSN,
                PondNum = pond.PondNum,
                PondArea = pond.PondArea,
                AreaName = pond.Area?.AreaName ?? "",
                TenantName = pond.Tenant?.TenantName ?? "",

                // 來自 FryRecord
                ManagerName = managerName,
                StockingCode = farmingCode,
                FishVarietyName = fishVarietyName,
                PLorGramUnit = plOrGram,
                StockingQty = stockingQty,
                Density = density,
                StockingDate = stockingDate,

                // 來自 Sampling / Harvest
                LastSampleOrHarvestDate = lastSampleOrHarvestDate,
                CurrentABW = currentABW,
                DOC = doc,

                // 來自 HarvestRecord + FryRecord
                AccumulatedHarvestWeight = totalHarvestWeight,
                HarvestTimes = harvestTimes,
                SurvivalRate = survivalRate,

                Remark = pond.Remark
            };
        }

    }
}
