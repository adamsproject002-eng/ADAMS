// Areas/AccountPermissionManagement/Controllers/FarmerManagementController.cs

using Microsoft.AspNetCore.Mvc;
using ADAMS.Attributes;
using ADAMS.Areas.AccountPermissionManagement.Services.FarmerManagement;
using ADAMS.Areas.AccountPermissionManagement.ViewModels.FarmerManagement;

namespace ADAMS.Areas.AccountPermissionManagement.Controllers
{
    [Area("AccountPermissionManagement")]
    [FunctionPermission(35)] // FunctionSN = 35 (養殖戶管理)
    public class FarmerManagementController : Controller
    {
        private readonly IFarmerService _farmerService;
        private readonly ILogger<FarmerManagementController> _logger;

        public FarmerManagementController(
            IFarmerService farmerService,
            ILogger<FarmerManagementController> logger)
        {
            _farmerService = farmerService;
            _logger = logger;
        }

        /// <summary>
        /// 養殖戶管理列表頁
        /// </summary>
        public async Task<IActionResult> Index(string? statusFilter, int page = 1 , int pageSize = 10 )
        {
            try
            {
                var viewModel = await _farmerService.GetPagedListAsync(statusFilter, page, pageSize);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "載入養殖戶列表失敗");
                TempData["ErrorMessage"] = "載入資料失敗，請稍後再試";
                return View(new FarmerListViewModel());
            }
        }

        /// <summary>
        /// GET: 新增養殖戶頁面
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            return View(new FarmerCreateViewModel());
        }

        /// <summary>
        /// POST: 新增養殖戶
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FarmerCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var currentUser = User.Identity?.Name ?? "System";
                var (success, message, farmerId) = await _farmerService.CreateAsync(model, currentUser);

                if (success)
                {
                    TempData["SuccessMessage"] = message;
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", message);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "新增養殖戶發生錯誤");
                ModelState.AddModelError("", "系統錯誤，請稍後再試");
                return View(model);
            }
        }

        /// <summary>
        /// GET: 編輯養殖戶頁面
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var detail = await _farmerService.GetDetailAsync(id);
                if (detail == null)
                {
                    TempData["ErrorMessage"] = "找不到該養殖戶";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = new FarmerEditViewModel
                {
                    SN = detail.SN,
                    TenantNum = detail.TenantNum,
                    TenantName = detail.TenantName,
                    TenantAddr = detail.TenantAddr,
                    ResponName = detail.ResponName,
                    ResponPhone = detail.ResponPhone,
                    ResponEmail = detail.ResponEmail,
                    IsEnable = detail.IsEnable
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

        /// <summary>
        /// POST: 更新養殖戶
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FarmerEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var currentUser = User.Identity?.Name ?? "System";
                var (success, message) = await _farmerService.UpdateAsync(model, currentUser);

                if (success)
                {
                    TempData["SuccessMessage"] = message;
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", message);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"更新養殖戶失敗 (SN: {model.SN})");
                ModelState.AddModelError("", "系統錯誤，請稍後再試");
                return View(model);
            }
        }

        /// <summary>
        /// GET: 查看養殖戶詳細資訊
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var viewModel = await _farmerService.GetDetailAsync(id);
                if (viewModel == null)
                {
                    TempData["ErrorMessage"] = "找不到該養殖戶";
                    return RedirectToAction(nameof(Index));
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"載入詳細資訊失敗 (ID: {id})");
                TempData["ErrorMessage"] = "載入資料失敗";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// POST: 切換養殖戶狀態（啟用/停用）
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id, bool isEnable)
        {
            try
            {
                var (success, message) = await _farmerService.ToggleStatusAsync(id, isEnable);
                return Json(new { success, message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"切換狀態失敗 (ID: {id})");
                return Json(new { success = false, message = "系統錯誤" });
            }
        }

        /// <summary>
        /// GET: API - 取得養殖戶下拉選單
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetFarmerList()
        {
            try
            {
                var list = await _farmerService.GetDropdownListAsync();
                return Json(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得養殖戶列表失敗");
                return Json(new List<FarmerDropdownDto>());
            }
        }
    }
}
