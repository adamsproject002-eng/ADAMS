using ADAMS.Areas.Models;
using ADAMS.Areas.OperationManagement.ViewModels.FeedingRecordQuery;
using ADAMS.Areas.PondOverview.ViewModels.PondOverviewPage;
using ADAMS.Data;
using ADAMS.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ADAMS.Areas.OperationManagement.Repositories.FeedingRecordQuery
{
    public class FeedingRecordRepository : IFeedingRecordRepository
    {
        private readonly AppDbContext _db;

        public FeedingRecordRepository(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// 養殖戶下拉選單
        /// </summary>
        public async Task<List<TenantOption>> GetTenantOptionsAsync(
            bool isOperationCompany,
            int currentTenantSN)
        {
            if (isOperationCompany)
            {
                return await _db.Tenant
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
                return await _db.Tenant
                    .Where(t => t.SN == currentTenantSN)
                    .Select(t => new TenantOption
                    {
                        SN = t.SN,
                        TenantName = t.TenantName
                    })
                    .ToListAsync();
            }
        }

        public async Task<List<FeedingRecordListItemViewModel>> SearchAsync(
            int tenantSN,
            int? areaSN,
            DateTime? startDate,
            DateTime? endDate)
        {
            var q = _db.FeedingRecord
                .Include(r => r.Pond)!.ThenInclude(p => p.Area)
                .Include(r => r.Feed)!.ThenInclude(f => f.Supplier)
                .Include(r => r.TimeZone)
                .Where(r => !r.IsDeleted && r.Pond!.TenantSN == tenantSN);

            if (areaSN.HasValue)
            {
                q = q.Where(r => r.Pond!.AreaSN == areaSN.Value);
            }

            if (startDate.HasValue)
                q = q.Where(r => r.FeedingDate >= startDate.Value);

            if (endDate.HasValue)
                q = q.Where(r => r.FeedingDate <= endDate.Value);

            var list = await q
                .OrderBy(r => r.FeedingDate)
                .ThenBy(r => r.FarmingCode)
                .Select((r) => new FeedingRecordListItemViewModel
                {
                    FeedingRecordSN = r.FeedingRecordSN,
                    FarmingCode = r.FarmingCode,
                    FeedingDate = r.FeedingDate,
                    TimeZoneText = r.TimeZone != null
                        ? $"{r.TimeZone.TimeZoneNum}" + $"({r.TimeZone.TimeZoneDesc})"
                        : "",
                    FeedBrand = r.Feed != null && r.Feed.Supplier != null
                        ? r.Feed.Supplier.SupplierName
                        : "",
                    FeedName = r.Feed != null ? r.Feed.FeedName : "",
                    FeedingAmount = r.FeedingAmount,
                    Unit = r.Unit,
                    SurvivalRate = r.SurvivalRate,
                    ABW = r.ABW,
                    DOC = r.DOC,
                    ABWGuide = null // 目前尚未有基準來源，先保留
                })
                .ToListAsync();

            return list;
        }

        public async Task<FeedingRecord?> GetAsync(int id)
        {
            return await _db.FeedingRecord
                .Include(r => r.Pond)!.ThenInclude(p => p.Area)
                .Include(r => r.Pond)!.ThenInclude(p => p.Tenant)
                .Include(r => r.Feed)!.ThenInclude(f => f.Supplier)
                .Include(r => r.TimeZone)
                .FirstOrDefaultAsync(r => r.FeedingRecordSN == id && !r.IsDeleted);
        }

        public async Task AddAsync(FeedingRecord entity)
        {
            _db.FeedingRecord.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(FeedingRecord entity)
        {
            _db.FeedingRecord.Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id, string userName)
        {
            var entity = await _db.FeedingRecord
                .FirstOrDefaultAsync(r => r.FeedingRecordSN == id);

            if (entity == null) return;

            entity.IsDeleted = true;
            entity.DeleteTime = DateTime.Now;
            entity.DeleteUser = userName;

            await _db.SaveChangesAsync();
        }

        public async Task<List<AreaOption>> GetAreaOptionsAsync(int tenantSN)
        {
            return await _db.Area
                .Where(a => a.TenantSN == tenantSN && !a.IsDeleted)
                .OrderBy(a => a.AreaName)
                .Select(a => new AreaOption
                {
                    AreaSN = a.AreaSN,
                    AreaName = a.AreaName
                })
                .ToListAsync();
        }

        public async Task<List<PondOption>> GetPondOptionsAsync(int tenantSN, int? areaSN)
        {
            var q = _db.Pond
                .Where(p => p.TenantSN == tenantSN && !p.IsDeleted);

            if (areaSN.HasValue)
                q = q.Where(p => p.AreaSN == areaSN.Value);

            return await q
                .OrderBy(p => p.PondNum)
                .Select(p => new PondOption
                {
                    PondSN = p.PondSN,
                    PondNum = p.PondNum
                })
                .ToListAsync();
        }

        public async Task<List<TimeZoneOptionVM>> GetTimeZoneOptionsAsync(int tenantSN)
        {
            return await _db.TimeZone
                .Where(t => t.TenantSN == tenantSN && !t.IsDeleted)
                .OrderBy(t => t.TimeZoneDesc)
                .Select(t => new TimeZoneOptionVM
                {
                    TimeZoneSN = t.TimeZoneSN,
                    DisplayText = $"{t.TimeZoneDesc}({t.TimeZoneNum})"
                })
                .ToListAsync();
        }

        public async Task<List<FeedOption>> GetFeedOptionsAsync(int tenantSN)
        {
            return await _db.Feed
                .Include(f => f.Supplier)
                .Where(f => f.TenantSN == tenantSN && !f.IsDeleted)
                .OrderBy(f => f.FeedName)
                .Select(f => new FeedOption
                {
                    FeedSN = f.FeedSN,
                    FeedName = f.FeedName,
                    UnitName = f.UnitName
                })
                .ToListAsync();
        }

        public async Task<List<ManageAccountOptionVM>> GetManageAccountOptionsAsync(int tenantSN)
        {
            return await _db.Account
                .Where(a => !a.IsDeleted && a.TenantSN == tenantSN && a.IsEnable)
                .OrderBy(a => a.AccountName)
                .Select(a => new ManageAccountOptionVM
                {
                    AccountName = a.AccountName,
                    RealName = a.RealName ?? a.AccountName
                })
                .ToListAsync();
        }

        public async Task<(string FarmingCode, string FishVarietyName, decimal ABW, int DOC, int StockingQty)?>
            GetLatestFarmingInfoAsync(int pondSN, DateTime feedingDate)
        {
            var targetDate = feedingDate.Date;
            // TODO: 這裡應該去查「放養紀錄 / 採樣紀錄」等資料表
            var fryRecord = await _db.FryRecord
                .Include(fr => fr.Fry)               // 對應 Fry 表
                    .ThenInclude(f => f.FishVariety)  //拿FishVariety 魚種資料
                .Where(fr => fr.PondSN == pondSN
                            && !fr.IsDeleted
                            && fr.FarmingDate <= targetDate)
                .OrderByDescending(fr => fr.FarmingDate) // 先照放苗日期由新到舊
                .ThenByDescending(fr => fr.FarmingNum)   // 同一天就看放養次數
                .FirstOrDefaultAsync();

            if (fryRecord == null)
            {
                // 這個池在指定日期之前完全沒有放苗資料 → 不帶預設值
                return null;
            }

            var fishVarietyName = fryRecord.Fry?.FishVariety?.FVName
                    ?? fryRecord.Fry?.FryName
                    ?? string.Empty;

            // 3. 找出同一個放養碼在 feedingDate 之前「最近一次投餌紀錄」，拿 ABW 用
            var latestFeeding = await _db.FeedingRecord
                .Where(fd => fd.PondSN == pondSN
                             && fd.FarmingCode == fryRecord.FarmingCode
                             && !fd.IsDeleted
                             && fd.FeedingDate <= feedingDate)
                .OrderByDescending(fd => fd.FeedingDate)
                .ThenByDescending(fd => fd.FeedingRecordSN)
                .FirstOrDefaultAsync();

            decimal abw;
            if (latestFeeding != null)
            {
                // 有投餌紀錄 → 用最近一次的 ABW 當預設
                abw = latestFeeding.ABW;
            }
            else
            {
                // 沒有投餌紀錄 → 用放苗時的 FryAge(克/尾) 當初始 ABW
                // 如果你的 FryAge 是 decimal?，這行改成：abw = fryRecord.FryAge ?? 0m;
                abw = fryRecord.FryAge;
            }

            // 4. 計算 DOC：從放苗日算到這次投餌日（含放苗當天）
            var doc = (int)(targetDate - fryRecord.FarmingDate.Date).TotalDays + 1;

            // 5. 放苗尾數（放苗表的 FarmingPCS）
            var stockingQty = fryRecord.FarmingPCS;

            return (
                FarmingCode: fryRecord.FarmingCode,
                FishVarietyName: fishVarietyName,
                ABW: abw,
                DOC: doc,
                StockingQty: stockingQty
            );
        }

        /// <summary>
        /// 取得飼料品牌(供應商)下拉：SupplierType = 1 才是飼料
        /// </summary>
        public async Task<List<SupplierOption>> GetFeedSuppliersAsync(int tenantSN)
        {
            return await _db.Supplier
                .Where(s => !s.IsDeleted &&
                            s.SupplierType == 1 &&        // 1 = 飼料
                            (s.TenantSN == tenantSN || tenantSN == 0))  // 營運公司帳號可看全部
                .OrderBy(s => s.SupplierName)
                .Select(s => new SupplierOption
                {
                    SupplierSN = s.SupplierSN,
                    SupplierName = s.SupplierName
                })
                .ToListAsync();
        }

        /// <summary>
        /// 依供應商取得飼料清單
        /// </summary>
        public async Task<List<FeedOption>> GetFeedsBySupplierAsync(int tenantSN, int supplierSN)
        {
            return await _db.Feed
                .Where(f => !f.IsDeleted &&
                            f.TenantSN == tenantSN &&
                            f.SupplierSN == supplierSN)
                .OrderBy(f => f.FeedName)
                .Select(f => new FeedOption
                {
                    FeedSN = f.FeedSN,
                    FeedName = f.FeedName,
                    UnitName = f.UnitName
                })
                .ToListAsync();
        }
    }
}
