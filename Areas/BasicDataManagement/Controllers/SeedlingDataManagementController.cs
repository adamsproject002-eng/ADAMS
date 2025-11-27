using ADAMS.Areas.BasicDataManagement.Services.SeedlingDataManagement;
using ADAMS.Areas.BasicDataManagement.ViewModels.SeedlingDataManagement;
using ADAMS.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace ADAMS.Areas.BasicDataManagement.Controllers
{
    [Area("BasicDataManagement")]
    [FunctionPermission(9)]  //SeedlingDataManagement
    public class SeedlingDataManagementController : Controller
    {
        private readonly ISeedlingDataService _service;
        private readonly ILogger<SeedlingDataManagementController> _logger;

        public SeedlingDataManagementController(
            ISeedlingDataService service,
            ILogger<SeedlingDataManagementController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // 列表
        [HttpGet]
        public async Task<IActionResult> Index(
            int? tenantSN,
            int selectedFVSN = 0,
            string? keyword = null,
            int page = 1,
            int pageSize = 10)
        {
            var vm = await _service.GetListViewModelAsync(tenantSN, selectedFVSN, keyword, page, pageSize);
            return View(vm);
        }

        // 新增 GET
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = await _service.GetCreateViewModelAsync();
            return View("Edit", vm);
        }

        // 新增 POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SeedlingEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // 重新載入下拉選單
                var vm = await _service.GetCreateViewModelAsync();
                vm.FryName = model.FryName;
                vm.UnitName = model.UnitName;
                vm.Remark = model.Remark;
                vm.SupplierSN = model.SupplierSN;
                vm.FVSN = model.FVSN;
                return View("Edit", vm);
            }

            try
            {
                await _service.CreateAsync(model);
                TempData["SuccessMessage"] = "新增魚苗資料成功";
                return RedirectToAction(nameof(Index), new { tenantSN = model.TenantSN });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "新增魚苗資料失敗");
                TempData["ErrorMessage"] = "新增失敗，請稍後再試";
                var vm = await _service.GetCreateViewModelAsync();
                return View("Edit", vm);
            }
        }

        // 修改 GET
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var vm = await _service.GetEditViewModelAsync(id);
            if (vm == null) return NotFound();
            return View(vm);
        }

        // 修改 POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SeedlingEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // 重新載入編輯用下拉
                var vm = await _service.GetEditViewModelAsync(model.FrySN ?? 0);
                if (vm == null) return NotFound();

                vm.FryName = model.FryName;
                vm.UnitName = model.UnitName;
                vm.Remark = model.Remark;
                vm.SupplierSN = model.SupplierSN;
                vm.FVSN = model.FVSN;

                return View(vm);
            }

            try
            {
                await _service.UpdateAsync(model);
                TempData["SuccessMessage"] = "修改魚苗資料成功";
                return RedirectToAction(nameof(Index), new { tenantSN = model.TenantSN });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "修改魚苗資料失敗");
                TempData["ErrorMessage"] = "修改失敗，請稍後再試";
                var vm = await _service.GetEditViewModelAsync(model.FrySN ?? 0);
                return View(vm ?? model);
            }
        }

        // 刪除（軟刪除）
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int tenantSN)
        {
            try
            {
                await _service.DeleteAsync(id);
                TempData["SuccessMessage"] = "刪除魚苗資料成功";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除魚苗資料失敗");
                TempData["ErrorMessage"] = "刪除失敗，請稍後再試";
            }

            return RedirectToAction(nameof(Index), new { tenantSN });
        }
    }
}
