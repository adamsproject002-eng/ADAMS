using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ADAMS.Attributes;
using ADAMS.Areas.OperationManagement.Services.StockingRecordQuery;
using ADAMS.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace ADAMS.Areas.OperationManagement.Controllers
{
    [Area("OperationManagement")]
    [FunctionPermission(21)]
    public class StockingRecordQueryController : Controller
    {
        private readonly FryRecordService _fryRecordService;

        private readonly AppDbContext _context;
        private readonly ILogger<StockingRecordQueryController> _logger;

        public StockingRecordQueryController(
            FryRecordService fryRecordService,
            ILogger<StockingRecordQueryController> logger
            , AppDbContext context)
        {
            _fryRecordService = fryRecordService;
            _logger = logger;
            _context = context;
        }

        // ==================== 列表頁 ====================
        public async Task<IActionResult> Index()
        {
            ViewBag.tenantSN = 1;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPagedListJson(
            int tenantSN = 1,
            int? pondSN = null,
            string? areaName = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int page = 1, int pageSize = 10)
        {
            var data = await _fryRecordService.GetPagedListDataAsync(tenantSN, pondSN, areaName, startDate, endDate, page, pageSize);
            return Json(data);
        }



        // GET: Create Page
        public async Task<IActionResult> Create()
        {
            int tenantSN = 1;
            ViewBag.TenantSN = tenantSN;
            ViewBag.CreateUser = "XXXXX";

            // 場區/池下拉選單
            ViewBag.AreaPondDropList = await _fryRecordService.GetCreateDropList(tenantSN);

            // 魚苗資料下拉選單
            ViewBag.FrySNDropList = await _fryRecordService.GetFryDropdownAsync(tenantSN);

            ViewBag.ManageAccountList = new String[] { "123", "456" };
            return View();
        }

        [HttpGet]
        // GET: Create Page
        public async Task<IActionResult> GetNextFarmingNum(string areaName, int pondSN)
        {
            // 查該池最大 FarmingNum
            var maxNum = await _context.FryRecord
                .Where(f => !f.IsDeleted && f.PondSN == pondSN)
                .MaxAsync(f => (int?)f.FarmingNum) ?? 0;

            int nextNum = maxNum + 1;

            // 放養碼 = 場區-池號 + Sxx
            string FarmingCode = $"{areaName}-{pondSN}S{nextNum:D2}";

            return Json(new
            {
                FarmingCode,
                FarmingNum = nextNum
            });
        }

        // // POST: 新增 JSON (直接用 object)
        [HttpPost]
        public async Task<IActionResult> CreateJson([FromBody] JsonElement model)
        {
            // JsonElement 是一個 Struct，不用判斷 null，判斷 ValueKind 即可
            if (model.ValueKind == JsonValueKind.Null || model.ValueKind == JsonValueKind.Undefined)
                return Json(new { success = false, message = "資料為空" });

            // 直接把 model 傳進去，Service 那邊改一下接法
            var result = await _fryRecordService.CreateFryRecordAsync(model);

            return Json(result);
        }



        // GET: Edit Page
        public async Task<IActionResult> Edit(int id)
        {
            int tenantSN = 1; // 假設 TenantSN
            ViewBag.TenantSN = tenantSN;
            ViewBag.ModifyUser = "Admin"; // 模擬當前登入者

            // 1. 取得該筆資料
            var record = await _fryRecordService.GetFryRecordByIdAsync(id);
            if (record == null)
            {
                return Content("查無資料");
            }

            // 將資料傳給 View (透過 Newtonsoft 序列化再轉成物件，或直接用 ViewBag)
            // 這裡為了方便 View 存取，我們把它轉成 dynamic 或直接傳 JSON 字串讓前端解析也可以
            // 為了配合你的風格，我直接把物件塞進 ViewBag
            ViewBag.EditData = record;

            // 2. 準備下拉選單 (場區/池、魚苗)
            ViewBag.AreaPondDropList = await _fryRecordService.GetCreateDropList(tenantSN);
            ViewBag.FrySNDropList = await _fryRecordService.GetFryDropdownAsync(tenantSN);
            ViewBag.ManageAccountList = new String[] { "123", "456" }; // 模擬管理員清單

            return View();
        }

        // POST: 修改 JSON
        [HttpPost]
        public async Task<IActionResult> UpdateJson([FromBody] JsonElement model)
        {
            if (model.ValueKind == JsonValueKind.Null || model.ValueKind == JsonValueKind.Undefined)
                return Json(new { success = false, message = "資料為空" });

            var result = await _fryRecordService.UpdateFryRecordAsync(model);

            return Json(result);
        }

        // POST: 軟刪除
        [HttpPost]
        public async Task<IActionResult> DisableJson([FromBody] JsonElement model)
        {
            // 檢查有沒有傳入 ID
            if (model.ValueKind == JsonValueKind.Null || !model.TryGetProperty("id", out var idProperty))
                return Json(new { success = false, message = "參數錯誤" });

            int id = idProperty.GetInt32();
            string modifyUser = "Admin"; // 模擬當前使用者

            var result = await _fryRecordService.DisableFryRecordAsync(id, modifyUser);

            return Json(result);
        }


    }
}


