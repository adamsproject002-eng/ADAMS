using ADAMS.Areas.BasicDataManagement.Services.SupplierDataManagement;
using ADAMS.Areas.BasicDataManagement.ViewModels.SupplierDataManagement;
using ADAMS.Attributes;
using ADAMS.Services;
using Microsoft.AspNetCore.Mvc;

namespace ADAMS.Areas.BasicDataManagement.Controllers
{
    [Area("BasicDataManagement")]
    [FunctionPermission(7)] // 假設 FunctionSN=7: 供應商資料管理（依你的 Function 表調整）
    public class SupplierDataManagementController : Controller
    {
        private readonly ISupplierDataService _service;
        private readonly ILogger<SupplierDataManagementController> _logger;
        private readonly ICurrentAccountService _current;

        public SupplierDataManagementController(
            ISupplierDataService service,
            ILogger<SupplierDataManagementController> logger,
            ICurrentAccountService current)
        {
            _service = service;
            _logger = logger;
            _current = current;
        }

        public async Task<IActionResult> Index(
            int? tenantSN,
            string? keyword,
            int supplierType = 0,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                var vm = await _service.GetListViewModelAsync(
                    tenantSN, keyword, supplierType, page, pageSize);
                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "載入供應商列表失敗");
                TempData["ErrorMessage"] = "載入資料失敗，請稍後再試";
                return View(new SupplierListViewModel());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? tenantSN)
        {
            var vm = await _service.GetCreateViewModelAsync(tenantSN);
            return View("Edit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // 重新取得下拉資料
                var vm = await _service.GetCreateViewModelAsync(model.TenantSN);
                vm.SupplierNum = model.SupplierNum;
                vm.SupplierName = model.SupplierName;
                vm.ContactName = model.ContactName;
                vm.ContactPhone = model.ContactPhone;
                vm.SupplierType = model.SupplierType;
                vm.Remark = model.Remark;

                return View("Edit", vm);
            }

            try
            {
                var currentUser = _current.Name ?? "system";
                await _service.CreateAsync(model, currentUser);

                TempData["SuccessMessage"] = "供應商已新增。";
                return RedirectToAction(nameof(Index), new { tenantSN = model.TenantSN });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "新增供應商失敗");
                TempData["ErrorMessage"] = "新增失敗，請稍後再試";
                return View("Edit", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var vm = await _service.GetEditViewModelAsync(id);
            if (vm == null)
            {
                TempData["ErrorMessage"] = "找不到供應商資料。";
                return RedirectToAction(nameof(Index));
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SupplierEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // 編輯時 TenantOptions 仍然需要
                var vm = await _service.GetEditViewModelAsync(model.SupplierSN!.Value);
                if (vm != null)
                {
                    vm.SupplierNum = model.SupplierNum;
                    vm.SupplierName = model.SupplierName;
                    vm.ContactName = model.ContactName;
                    vm.ContactPhone = model.ContactPhone;
                    vm.SupplierType = model.SupplierType;
                    vm.Remark = model.Remark;
                    return View(vm);
                }

                return View(model);
            }

            try
            {
                var currentUser = _current.Name ?? "system";
                await _service.UpdateAsync(model, currentUser);

                TempData["SuccessMessage"] = "供應商已更新。";
                return RedirectToAction(nameof(Index), new { tenantSN = model.TenantSN });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新供應商失敗");
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
                var currentUser = _current.Name ?? "system";
                await _service.SoftDeleteAsync(id, currentUser);

                TempData["SuccessMessage"] = "供應商已刪除。";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除供應商失敗 SupplierSN={Id}", id);
                TempData["ErrorMessage"] = "刪除失敗，請稍後再試";
            }

            return RedirectToAction(nameof(Index), new { tenantSN });
        }
    }
}
