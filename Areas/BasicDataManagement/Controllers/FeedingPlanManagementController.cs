using ADAMS.Areas.BasicDataManagement.Services.FeedingPlanManagement;
using ADAMS.Areas.BasicDataManagement.ViewModels.FeedingPlanManagement;
using ADAMS.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace ADAMS.Areas.BasicDataManagement.Controllers
{
    [Area("BasicDataManagement")]
    [FunctionPermission(13)] //投餌規劃管理
    public class FeedingPlanManagementController : Controller
    {
        private readonly IFeedingPlanService _service;

        public FeedingPlanManagementController(IFeedingPlanService service)
        {
            _service = service;
        }

        // GET: /BasicDataManagement/FeedingPlanManagement
        [HttpGet]
        public async Task<IActionResult> Index(int? tenantSN, int? fvsn)
        {
            var vm = await _service.GetListViewModelAsync(tenantSN, fvsn);
            return View(vm);
        }

        // GET: Create
        [HttpGet]
        public async Task<IActionResult> Create(int? tenantSN, int? fvsn)
        {
            var vm = await _service.GetCreateViewModelAsync(tenantSN, fvsn);
            return View("Edit", vm);
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FeedingPlanEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", model);
            }

            await _service.CreateAsync(model);
            TempData["SuccessMessage"] = "新增投餌規劃資料成功。";
            return RedirectToAction(nameof(Index), new { tenantSN = model.TenantSN, fvsn = model.FVSN });
        }

        // GET: Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var vm = await _service.GetEditViewModelAsync(id);
            if (vm == null) return NotFound();

            return View(vm);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FeedingPlanEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _service.UpdateAsync(model);
            TempData["SuccessMessage"] = "修改投餌規劃資料成功。";
            return RedirectToAction(nameof(Index), new { tenantSN = model.TenantSN, fvsn = model.FVSN });
        }

        // POST: Delete (Soft Delete)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int tenantSN, int fvsn)
        {
            await _service.SoftDeleteAsync(id);
            TempData["SuccessMessage"] = "刪除投餌規劃資料成功。";
            return RedirectToAction(nameof(Index), new { tenantSN, fvsn });
        }
    }
}
