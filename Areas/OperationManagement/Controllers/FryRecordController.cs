using Microsoft.AspNetCore.Mvc;
using ADAMS.Attributes;
using ADAMS.Areas.OperationManagement.Services.FryRecords;
using ADAMS.Areas.OperationManagement.ViewModels.FryRecords;

namespace ADAMS.Areas.OperationManagement.Controllers
{
    [Area("OperationManagement")]
    [FunctionPermission(21)]
    public class FryRecordController : Controller
    {
        private readonly IFryRecordService _fryRecordService;
        private readonly ILogger<FryRecordController> _logger;

        public FryRecordController(
            IFryRecordService fryRecordService,
            ILogger<FryRecordController> logger)
        {
            _fryRecordService = fryRecordService;
            _logger = logger;
        }

        // ==================== 列表頁 ====================
        public async Task<IActionResult> Index(
            int tenantSN,
            int? areaSN = null,
            int? pondSN = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                var viewModel = await _fryRecordService.GetPagedListAsync(
                    tenantSN, areaSN, pondSN, startDate, endDate, page, pageSize);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "載入放苗紀錄列表失敗");
                TempData["ErrorMessage"] = "載入資料失敗，請稍後再試";
                return View(new FryRecordListViewModel());
            }
        }

        // ==================== 新增頁面 ====================
        [HttpGet]
        public IActionResult Create()
        {
            return View(new FryRecordCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FryRecordCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var currentUser = User.Identity?.Name ?? "System";
                var (success, message, fryRecordSN) = await _fryRecordService.CreateAsync(model, currentUser);

                if (success)
                {
                    TempData["SuccessMessage"] = message;
                    return RedirectToAction(nameof(Index), new { tenantSN = model.PondSN });
                }

                ModelState.AddModelError("", message);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "新增放苗紀錄發生錯誤");
                ModelState.AddModelError("", "系統錯誤，請稍後再試");
                return View(model);
            }
        }

        // ==================== 編輯頁面 ====================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var detail = await _fryRecordService.GetDetailAsync(id);
                if (detail == null)
                {
                    TempData["ErrorMessage"] = "找不到該放苗紀錄";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = new FryRecordEditViewModel
                {
                    FryRecordSN = detail.FryRecordSN,
                    PondSN = 0, // 可根據需求抓 PondSN
                    FarmingNum = detail.FarmingNum,
                    FarmingCode = detail.FarmingCode,
                    FarmingDate = detail.FarmingDate,
                    FarmingPCS = detail.FarmingPCS,
                    FryAge = detail.FryAge,
                    PondArea = detail.PondArea,
                    FarmingDensity = detail.FarmingDensity,
                    ManageAccount = detail.ManageAccount,
                    Remark = detail.Remark
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"載入編輯頁面失敗 (ID: {id})");
                TempData["ErrorMessage"] = "載入資料失敗";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FryRecordEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var currentUser = User.Identity?.Name ?? "System";
                var (success, message) = await _fryRecordService.UpdateAsync(model, currentUser);

                if (success)
                {
                    TempData["SuccessMessage"] = message;
                    return RedirectToAction(nameof(Index), new { tenantSN = model.PondSN });
                }

                ModelState.AddModelError("", message);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"更新放苗紀錄失敗 (SN: {model.FryRecordSN})");
                ModelState.AddModelError("", "系統錯誤，請稍後再試");
                return View(model);
            }
        }

        // ==================== 詳細頁面 ====================
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var detail = await _fryRecordService.GetDetailAsync(id);
                if (detail == null)
                {
                    TempData["ErrorMessage"] = "找不到該放苗紀錄";
                    return RedirectToAction(nameof(Index));
                }

                return View(detail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"載入詳細頁面失敗 (ID: {id})");
                TempData["ErrorMessage"] = "載入資料失敗";
                return RedirectToAction(nameof(Index));
            }
        }

        // ==================== 軟刪除 ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                var currentUser = User.Identity?.Name ?? "System";
                var (success, message) = await _fryRecordService.SoftDeleteAsync(id, currentUser);
                return Json(new { success, message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"刪除放苗紀錄失敗 (ID: {id})");
                return Json(new { success = false, message = "系統錯誤" });
            }
        }

        // ==================== 下拉選單 API ====================
        [HttpGet]
        public async Task<IActionResult> GetFryRecordDropdown(int tenantSN)
        {
            try
            {
                var list = await _fryRecordService.GetDropdownListAsync(tenantSN);
                return Json(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得放苗紀錄下拉選單失敗");
                return Json(new List<FryRecordDropdownDto>());
            }
        }
    }
}
