using ADAMS.Data;
using Microsoft.EntityFrameworkCore;
using ADAMS.Models;
using Newtonsoft.Json.Linq;


namespace ADAMS.Areas.OperationManagement.Services.StockingRecordQuery
{
    public class FryRecordService
    {

        private readonly ILogger<FryRecordService> _logger;
        private readonly AppDbContext _context;

        public FryRecordService(ILogger<FryRecordService> logger, AppDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        // ==================== 分頁列表 ====================
        public async Task<object> GetPagedListDataAsync(
            int tenantSN,
            int? pondSN = null,
            string? areaName = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int page = 1,
            int pageSize = 20)
        {
            // 取得資料
            var query = _context.FryRecord
                .Include(f => f.Pond)
                .ThenInclude(p => p.Area)
                .Where(f => !f.IsDeleted && f.Pond.TenantSN == tenantSN)
                .AsNoTracking();

            if (areaName != null)
                query = query.Where(f => f.Pond.Area.AreaName == areaName);

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

            // 將資料轉成匿名型別
            var records = data.Select(f => new
            {
                f.FryRecordSN,
                AreaName = f.Pond.Area.AreaName,
                f.PondSN,
                f.FarmingNum,
                f.FarmingCode,
                f.FarmingDate,
                f.FarmingPCS,
                f.FryAge,
                f.PondArea,
                f.FarmingDensity,
                f.ManageAccount,
                f.Remark,
                f.FryType,
                f.FrySource,
                f.CreateTime,
                f.CreateUser,
                f.ModifyTime,
                f.ModifyUser,
                f.IsDeleted
            }).ToList();
            //放苗作業紀錄廠區下拉列表
            //查出該 tenantSN 所有對應的 f.Pond.Area.AreaName,
            var areaNames = await _context.FryRecord
                .Include(f => f.Pond)
                .ThenInclude(p => p.Area)
                .Where(f => !f.IsDeleted && f.Pond.TenantSN == tenantSN)
                .Select(f => f.Pond.Area.AreaName)
                .Distinct()
                .ToListAsync();

            // 包成 object 回傳 
            return new
            {
                Records = records,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                AreaNames = areaNames,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }


        public async Task<Dictionary<string, List<int>>> GetCreateDropList(int tenantSN)
        {
            var query = _context.FryRecord
                .Include(f => f.Pond)
                .ThenInclude(p => p.Area)
                .Where(f => !f.IsDeleted && f.Pond.TenantSN == tenantSN)
                .AsNoTracking();

            // 取得所有廠區+池號，去重
            var list = await query
                .Select(f => new
                {
                    AreaName = f.Pond.Area.AreaName,
                    PondSN = f.PondSN
                })
                .Distinct()
                .ToListAsync();

            // 分組成 Dictionary<string, List<int>>
            var result = list
                .GroupBy(x => x.AreaName)
                .ToDictionary(g => g.Key, g => g.Select(x => x.PondSN).ToList());

            return result;
        }


        public async Task<List<object>> GetFryDropdownAsync(int tenantSN)
        {
            // 取得該養殖戶的魚苗資料，包含 Supplier
            var list = await _context.Fry
                .Include(f => f.Supplier)
                .Where(f => !f.IsDeleted && f.TenantSN == tenantSN)
                .Select(f => new
                {
                    FrySN = f.FrySN,
                    FryName = f.FryName,
                    FVName = f.FVName,
                    SupplierName = f.Supplier.SupplierName
                })
                .ToListAsync<object>();

            return list;
        }

        public async Task<object> CreateFryRecordAsync(object model)
        {
            try
            {
                // 因為 model 現在是 System.Text.Json.JsonElement
                // 直接 .ToString() 會得到原本的 JSON 字串
                // 然後用 Parse 來轉成 JObject
                JObject data = JObject.Parse(model.ToString());
                Console.WriteLine("data: {0}", data.ToString());
                //根據FrySN
                Console.WriteLine($"PondSN: {data["PondSN"]}");
                Console.WriteLine($"FarmingNum: {data["FarmingNum"]}");
                Console.WriteLine($"FarmingCode: {data["FarmingCode"]}");
                Console.WriteLine($"FrySN: {data["FrySN"]}");
                Console.WriteLine($"FarmingDate: {data["FarmingDate"]}");
                Console.WriteLine($"FarmingPCS: {data["FarmingPCS"]}");
                Console.WriteLine($"FryAge: {data["FryAge"]}");
                Console.WriteLine($"PondArea: {data["PondArea"]}");
                Console.WriteLine($"FarmingDensity: {data["FarmingDensity"]}");
                Console.WriteLine($"ManageAccount: {data["CreateUser"]}");
                Console.WriteLine($"Remark: {data["Remark"]}");
                Console.WriteLine($"CreateUser: {data["CreateUser"]}");

                var entity = new FryRecord
                {
                    PondSN = (int)data["PondSN"],
                    FarmingNum = (int)data["FarmingNum"],
                    FarmingCode = (string)data["FarmingCode"],
                    FrySN = (int)data["FrySN"],
                    FarmingDate = DateTime.Parse((string)data["FarmingDate"]),
                    FarmingPCS = (int)data["FarmingPCS"],
                    FryAge = (decimal)data["FryAge"],
                    PondArea = (decimal)data["PondArea"],
                    FarmingDensity = (decimal)data["FarmingDensity"],
                    ManageAccount = (string)data["CreateUser"],
                    Remark = (string?)data["Remark"],
                    IsDeleted = false,
                    CreateTime = DateTime.Now,
                    CreateUser = (string)data["CreateUser"],
                };

                _context.FryRecord.Add(entity);
                await _context.SaveChangesAsync();

                return new
                {
                    success = true,
                    message = "新增成功",
                    fryRecordSN = entity.FryRecordSN
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating FryRecord");
                Console.WriteLine(ex.ToString());
                return new
                {
                    success = false,
                    message = ex.Message,
                    // details = ex.StackTrace // 加入詳細的錯誤資訊
                };
            }
        }


        // ==================== 取得單筆資料 (Edit 用) ====================
        public async Task<object> GetFryRecordByIdAsync(int fryRecordSN)
        {
            var record = await _context.FryRecord
                .Include(f => f.Pond)
                .ThenInclude(p => p.Area)
                .Include(f => f.Fry) // 關聯魚苗名稱
                .ThenInclude(fry => fry.Supplier) // 關聯供應商
                .Where(f => f.FryRecordSN == fryRecordSN)
                .Select(f => new
                {
                    f.FryRecordSN,
                    f.PondSN,
                    AreaName = f.Pond.Area.AreaName,
                    f.FarmingNum,
                    f.FarmingCode,
                    // 轉成字串格式 yyyy-MM-dd 方便前端 input type="date" 使用
                    FarmingDate = f.FarmingDate.ToString("yyyy-MM-dd"),
                    f.FarmingPCS,
                    f.FryAge,
                    f.PondArea,
                    f.FarmingDensity,
                    f.ManageAccount,
                    f.Remark,
                    f.FrySN,
                    FryName = f.Fry.FryName,
                    FVName = f.Fry.FVName,
                    SupplierName = f.Fry.Supplier.SupplierName,
                    f.CreateUser
                })
                .FirstOrDefaultAsync();

            return record;
        }

        // ==================== 修改資料 (Update) ====================
        public async Task<object> UpdateFryRecordAsync(object model)
        {
            try
            {
                // 解析 JSON
                JObject data = JObject.Parse(model.ToString());

                int fryRecordSN = (int)data["FryRecordSN"];

                // 撈出原始資料
                var entity = await _context.FryRecord.FindAsync(fryRecordSN);

                if (entity == null)
                {
                    return new { success = false, message = "找不到該筆資料" };
                }

                // 更新欄位 (注意：FarmingCode 不更新)
                // PondSN 若允許修改則更新，若不允許則註解掉
                entity.PondSN = (int)data["PondSN"];
                // entity.FarmingNum 不建議修改，因為會影響 Code 生成邏輯

                entity.FrySN = (int)data["FrySN"];
                entity.FarmingDate = DateTime.Parse(data["FarmingDate"].ToString());
                entity.FarmingPCS = (int)data["FarmingPCS"];
                entity.FryAge = (decimal)data["FryAge"];
                entity.PondArea = (decimal)data["PondArea"];
                entity.FarmingDensity = (decimal)data["FarmingDensity"];

                // 修改管理員 (UI 傳來的是 ManagerAccount)
                entity.ManageAccount = (string)data["ManagerAccount"];

                entity.Remark = (string?)data["Remark"];

                // 紀錄修改資訊
                entity.ModifyTime = DateTime.Now;
                entity.ModifyUser = (string)data["ModifyUser"]; // 從 Controller 傳進來

                await _context.SaveChangesAsync();

                return new
                {
                    success = true,
                    message = "修改成功"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating FryRecord");
                return new
                {
                    success = false,
                    message = "修改失敗：" + ex.Message
                };
            }
        }

        // ==================== 刪除 (軟刪除) ====================
        public async Task<object> DisableFryRecordAsync(int fryRecordSN, string modifyUser)
        {
            try
            {
                var entity = await _context.FryRecord.FindAsync(fryRecordSN);

                if (entity == null)
                {
                    return new { success = false, message = "找不到該筆資料" };
                }

                // 執行軟刪除
                entity.IsDeleted = true;
                entity.ModifyTime = DateTime.Now;
                entity.ModifyUser = modifyUser;

                await _context.SaveChangesAsync();

                return new
                {
                    success = true,
                    message = "刪除成功"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while disabling FryRecord");
                return new
                {
                    success = false,
                    message = "刪除失敗：" + ex.Message
                };
            }
        }


    }
}
