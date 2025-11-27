using ADAMS.Areas.BasicDataManagement.Services.FeedDataManagement;
using ADAMS.Areas.BasicDataManagement.ViewModels.FeedDataManagement;
using ADAMS.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace ADAMS.Areas.BasicDataManagement.Controllers
{
    [Area("BasicDataManagement")]
    [FunctionPermission(10)] //飼料資料管理
    public class FeedDataManagementController : Controller
    {
        private readonly IFeedDataService _service;

        public FeedDataManagementController(IFeedDataService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            int? tenantSN,
            string? keyword,
            int page = 1,
            int pageSize = 10)
        {
            var vm = await _service.GetListViewModelAsync(tenantSN, keyword, page, pageSize);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = await _service.GetCreateViewModelAsync();
            return View("Edit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FeedEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // 重新補齊下拉資料
                var vm = await _service.GetCreateViewModelAsync();
                vm.FeedName = model.FeedName;
                vm.UnitName = model.UnitName;
                vm.Remark = model.Remark;
                vm.SupplierSN = model.SupplierSN;
                vm.TenantSN = model.TenantSN;
                return View("Edit", vm);
            }

            await _service.CreateAsync(model);
            TempData["SuccessMessage"] = "新增飼料資料成功。";
            return RedirectToAction("Index", new { tenantSN = model.TenantSN });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var vm = await _service.GetEditViewModelAsync(id);
            if (vm == null) return NotFound();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FeedEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // 重新補齊下拉資料
                var vm = await _service.GetEditViewModelAsync(model.FeedSN!.Value);
                if (vm == null) return NotFound();

                vm.FeedName = model.FeedName;
                vm.UnitName = model.UnitName;
                vm.Remark = model.Remark;
                vm.SupplierSN = model.SupplierSN;
                return View(vm);
            }

            var ok = await _service.UpdateAsync(model);
            if (!ok)
            {
                TempData["ErrorMessage"] = "更新失敗，資料不存在。";
                return RedirectToAction("Index", new { tenantSN = model.TenantSN });
            }

            TempData["SuccessMessage"] = "更新飼料資料成功。";
            return RedirectToAction("Index", new { tenantSN = model.TenantSN });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int tenantSN)
        {
            await _service.SoftDeleteAsync(id);
            TempData["SuccessMessage"] = "刪除飼料資料成功。";
            return RedirectToAction("Index", new { tenantSN });
        }
    }
}
