using ADAMS.Areas.BasicDataManagement.Services.GrowthTargetReference;
using ADAMS.Areas.BasicDataManagement.ViewModels.GrowthTargetReference;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ADAMS.Areas.BasicDataManagement.Controllers
{
    [Area("BasicDataManagement")]
    [Authorize]
    public class GrowthTargetReferenceController : Controller
    {
        private readonly IGrowthTargetService _service;

        public GrowthTargetReferenceController(IGrowthTargetService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? fvsn)
        {
            var vm = await _service.GetListViewModelAsync(fvsn);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int fvsn)
        {
            var vm = await _service.GetCreateViewModelAsync(fvsn);
            if (!vm.IsOperationCompany)
                return Forbid();

            return View("Edit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GrowthTargetEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Edit", model);

            await _service.CreateAsync(model);
            TempData["SuccessMessage"] = "新增成長目標參考值成功";

            return RedirectToAction(nameof(Index), new { fvsn = model.FVSN });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var vm = await _service.GetEditViewModelAsync(id);
            if (vm == null)
                return NotFound();

            if (!vm.IsOperationCompany)
                return Forbid();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GrowthTargetEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _service.UpdateAsync(model);
            TempData["SuccessMessage"] = "修改成長目標參考值成功";

            return RedirectToAction(nameof(Index), new { fvsn = model.FVSN });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int fvsn)
        {
            await _service.SoftDeleteAsync(id);
            TempData["SuccessMessage"] = "刪除成長目標參考值成功";
            return RedirectToAction(nameof(Index), new { fvsn });
        }
    }
}
