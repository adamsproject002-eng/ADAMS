using ADAMS.Areas.AccountPermissionManagement.Services.GroupPermissionManagement;
using ADAMS.Areas.AccountPermissionManagement.ViewModels.GroupPermissionManagement;
using ADAMS.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace ADAMS.Areas.AccountPermissionManagement.Controllers
{
    [Area("AccountPermissionManagement")]
    [FunctionPermission(34)] // FunctionSN = 34 (群組權限管理)
    public class GroupPermissionManagementController : Controller
    {
        private readonly IGroupPermissionService _groupPermissionService;
        private readonly ILogger<GroupPermissionManagementController> _logger;

        public GroupPermissionManagementController(
            IGroupPermissionService groupPermissionService,
            ILogger<GroupPermissionManagementController> logger)
        {
            _groupPermissionService = groupPermissionService;
            _logger = logger;
        }
        /// <summary>
        /// 群組權限列表頁
        /// </summary>
        public async Task<IActionResult> Index(int tenantSN, string? groupName, int page = 1, int pageSize = 10)
        {
            try
            {
                var viewModel = await _groupPermissionService.GetPagedListAsync(tenantSN, groupName, page, pageSize);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "載入群組權限列表失敗");
                TempData["ErrorMessage"] = "載入資料失敗，請稍後再試";
                return View(new GroupPermissionListViewModel());
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAccountListByGroup(int id)
        {
            var viewModel = await _groupPermissionService.GetAccountsByGroupAsync(id);
            return View(viewModel);
        }

        /// <summary>
        /// 查看某群組的功能權限
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ViewPermissions(int id)
        {
            try
            {
                var viewModel = await _groupPermissionService.GetPermissionsByGroupAsync(id);
                return View(viewModel);        // 對應下面要新增的 View
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "載入群組權限明細失敗，AccountGroupSN={AccountGroupSN}", id);
                TempData["ErrorMessage"] = "載入群組權限失敗，請稍後再試";
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = await _groupPermissionService.GetCreateViewModelAsync();
            return View("Edit", vm); // 共用 Edit.cshtml
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GroupPermissionEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // 驗證失敗：重新載入下拉與功能清單，再把使用者勾選覆蓋回去
                var vm = await _groupPermissionService.GetCreateViewModelAsync();
                vm.Name = model.Name;
                vm.Remark = model.Remark;
                vm.TenantSN = model.TenantSN;

                foreach (var f in vm.Functions)
                {
                    var m = model.Functions.FirstOrDefault(x => x.FunctionSN == f.FunctionSN);
                    if (m != null)
                    {
                        f.IsChecked = m.IsChecked;
                    }
                }

                return View("Edit", vm);
            }

            try
            {
                await _groupPermissionService.SaveGroupAsync(model);

                TempData["SuccessMessage"] = "群組已新增。";
                return RedirectToAction(nameof(Index), new { tenantSN = model.TenantSN });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "新增群組失敗");
                TempData["ErrorMessage"] = "新增失敗，請稍後再試";
                ModelState.AddModelError(nameof(model.Name), ex.Message);
                return View("Edit", model);
            }
        }

        // ---------- 修改群組 ----------

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var vm = await _groupPermissionService.GetEditViewModelAsync(id);
            return View(vm); // Edit.cshtml
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GroupPermissionEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _groupPermissionService.SaveGroupAsync(model);

                TempData["SuccessMessage"] = "群組已更新。";
                return RedirectToAction(nameof(Index), new { tenantSN = model.TenantSN });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新群組失敗");
                ModelState.AddModelError(nameof(model.Name), ex.Message);
                TempData["ErrorMessage"] = "更新失敗，請稍後再試";
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int tenantSN)
        {
            try
            {
                await _groupPermissionService.DeleteGroupAsync(id);

                TempData["SuccessMessage"] = "群組已刪除。";
            }
            catch (InvalidOperationException ex)
            {
                // 業務邏輯錯誤（例如：Admin 群組 / 有帳號歸屬）
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除群組失敗，AccountGroupSN={Id}", id);
                TempData["ErrorMessage"] = "刪除失敗，請稍後再試。";
            }

            // 保留當前養殖戶的查詢條件
            return RedirectToAction(nameof(Index), new { tenantSN });
        }
    }
}
