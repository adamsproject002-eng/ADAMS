using ADAMS.Areas.AccountPermissionManagement.Services.UserAccountManagement;
using ADAMS.Areas.AccountPermissionManagement.ViewModels.UserAccountManagement;
using ADAMS.Attributes;
using ADAMS.Services;
using Microsoft.AspNetCore.Mvc;

namespace ADAMS.Areas.AccountPermissionManagement.Controllers
{
    [Area("AccountPermissionManagement")]
    [FunctionPermission(33)] //使用者帳號管理
    public class UserAccountManagementController : Controller
    {
        private readonly IUserAccountService _userService;
        private readonly ICurrentAccountService _current;
        private readonly ILogger<UserAccountManagementController> _logger;

        public UserAccountManagementController(
            IUserAccountService userService,
            ICurrentAccountService current,
            ILogger<UserAccountManagementController> logger)
        {
            _userService = userService;
            _current = current;
            _logger = logger;
        }

        /// <summary>
        /// 使用者清單
        /// </summary>
        public async Task<IActionResult> Index(
            int? tenantSN,
            string? keyword,
            int status = 0,    // 0=全部,1=啟用,2=停用
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                var vm = await _userService.GetListViewModelAsync(
                    tenantSN,
                    keyword,
                    status,
                    page,
                    pageSize);

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "載入使用者帳號列表失敗");
                TempData["ErrorMessage"] = "載入資料失敗，請稍後再試。";
                return View(new UserAccountListViewModel());
            }
        }

        /// <summary>
        /// 依養殖戶 SN 取得帳號群組清單（給前端下拉 Ajax 用）
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAccountGroups(int tenantSN)
        {
            var groups = await _userService.GetAccountGroupsForTenantAsync(tenantSN);
            return Json(groups);
        }

        /// <summary>
        /// 新增使用者（畫面）
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create(int? tenantSN)
        {
            var vm = await _userService.GetCreateViewModelAsync(tenantSN);
            return View("Edit", vm);
        }

        /// <summary>
        /// 新增使用者（送出）
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserAccountEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var vmInvalid = await _userService.RebuildEditViewModelAsync(model);
                return View("Edit", vmInvalid);
            }

            try
            {
                await _userService.CreateAsync(model);
                TempData["SuccessMessage"] = "使用者帳號已新增。";
                return RedirectToAction(nameof(Index), new { tenantSN = model.TenantSN });
            }
            catch (InvalidOperationException ex)
            {
                // 可預期的業務錯誤（例如帳號重複）
                _logger.LogWarning(ex, "新增使用者帳號失敗（商業規則）");
                ModelState.AddModelError(string.Empty, ex.Message);

                var vm = await _userService.RebuildEditViewModelAsync(model);
                return View("Edit", vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "新增使用者帳號發生未預期錯誤");
                TempData["ErrorMessage"] = "新增失敗，請稍後再試。";

                var vm = await _userService.RebuildEditViewModelAsync(model);
                return View("Edit", vm);
            }
        }

        /// <summary>
        /// 修改使用者（畫面）
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var vm = await _userService.GetEditViewModelAsync(id);
            if (vm == null)
            {
                TempData["ErrorMessage"] = "找不到指定的使用者帳號。";
                return RedirectToAction(nameof(Index));
            }

            return View(vm);
        }

        /// <summary>
        /// 修改使用者（送出）
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserAccountEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var vmInvalid = await _userService.RebuildEditViewModelAsync(model);
                return View(vmInvalid);
            }

            try
            {
                await _userService.UpdateAsync(model);
                TempData["SuccessMessage"] = "使用者帳號已更新。";
                return RedirectToAction(nameof(Index), new { tenantSN = model.TenantSN });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "更新使用者帳號失敗（商業規則）");
                ModelState.AddModelError(string.Empty, ex.Message);

                var vm = await _userService.RebuildEditViewModelAsync(model);
                return View("Edit", vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新使用者帳號發生未預期錯誤");
                TempData["ErrorMessage"] = "更新失敗，請稍後再試。";

                var vm = await _userService.RebuildEditViewModelAsync(model);
                return View("Edit", vm);
            }
        }

        /// <summary>
        /// 啟用 / 停用 帳號（切換）
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleEnable(int id, int tenantSN)
        {
            try
            {
                await _userService.ToggleEnableAsync(id);
                TempData["SuccessMessage"] = "帳號啟用狀態已更新。";
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "切換帳號啟用狀態失敗（商業規則）");
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "切換帳號啟用狀態發生未預期錯誤");
                TempData["ErrorMessage"] = "更新失敗，請稍後再試。";
            }

            return RedirectToAction(nameof(Index), new { tenantSN });
        }

        /// <summary>
        /// 軟刪除使用者
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int tenantSN)
        {
            try
            {
                await _userService.SoftDeleteAsync(id);
                TempData["SuccessMessage"] = "使用者帳號已刪除。";
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "刪除使用者帳號失敗（商業規則）");
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除使用者帳號發生未預期錯誤");
                TempData["ErrorMessage"] = "刪除失敗，請稍後再試。";
            }

            return RedirectToAction(nameof(Index), new { tenantSN });
        }
    }
}
