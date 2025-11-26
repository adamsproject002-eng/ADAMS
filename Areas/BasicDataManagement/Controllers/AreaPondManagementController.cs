using ADAMS.Areas.BasicDataManagement.Services.AreaPondManagement;
using ADAMS.Areas.BasicDataManagement.ViewModels.AreaPondManagement;
using ADAMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ADAMS.Areas.BasicDataManagement.Controllers
{
    [Area("BasicDataManagement")]
    [Authorize]
    public class AreaPondManagementController : Controller
    {
        private readonly IAreaPondService _service;
        private readonly ICurrentAccountService _current;

        public AreaPondManagementController(
            IAreaPondService service,
            ICurrentAccountService current)
        {
            _service = service;
            _current = current;
        }

        // ---------- 場區清單 ----------

        public async Task<IActionResult> Index(
            int? tenantSN,
            string? keyword,
            int page = 1,
            int pageSize = 10)
        {
            var vm = await _service.GetAreaListViewModelAsync(tenantSN, keyword, page, pageSize);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> CreateArea(int? tenantSN)
        {
            var vm = await _service.GetAreaEditViewModelAsync(null, tenantSN);
            return View("AreaEdit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateArea(AreaEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var vm = await _service.GetAreaEditViewModelAsync(null, model.TenantSN);
                vm.AreaName = model.AreaName;
                vm.GPSX = model.GPSX;
                vm.GPSY = model.GPSY;
                vm.Remark = model.Remark;
                return View("AreaEdit", vm);
            }

            await _service.CreateAreaAsync(model, _current.Name);
            TempData["SuccessMessage"] = "新增養殖區成功";
            return RedirectToAction(nameof(Index), new { tenantSN = model.TenantSN });
        }

        [HttpGet]
        public async Task<IActionResult> EditArea(int id)
        {
            var vm = await _service.GetAreaEditViewModelAsync(id, null);
            return View("AreaEdit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditArea(AreaEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var vm = await _service.GetAreaEditViewModelAsync(model.AreaSN, model.TenantSN);
                vm.AreaName = model.AreaName;
                vm.GPSX = model.GPSX;
                vm.GPSY = model.GPSY;
                vm.Remark = model.Remark;
                return View("AreaEdit", vm);
            }

            await _service.UpdateAreaAsync(model, _current.Name);
            TempData["SuccessMessage"] = "修改養殖區成功";
            return RedirectToAction(nameof(Index), new { tenantSN = model.TenantSN });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteArea(int id, int tenantSN)
        {
            try
            {
                await _service.DeleteAreaAsync(id, _current.Name);
                TempData["SuccessMessage"] = "刪除養殖區成功";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { tenantSN });
        }

        // ---------- 養殖池清單 ----------

        [HttpGet]
        public async Task<IActionResult> PondList(
            int areaSN,
            string? keyword,
            int page = 1,
            int pageSize = 10)
        {
            var vm = await _service.GetPondListViewModelAsync(areaSN, keyword, page, pageSize);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> CreatePond(int areaSN)
        {
            var vm = await _service.GetPondEditViewModelAsync(null, areaSN);
            return View("PondEdit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePond(PondEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var vm = await _service.GetPondEditViewModelAsync(null, model.AreaSN);
                vm.PondNum = model.PondNum;
                vm.PondWidth = model.PondWidth;
                vm.PondLength = model.PondLength;
                vm.PondArea = model.PondArea;
                vm.GPSX = model.GPSX;
                vm.GPSY = model.GPSY;
                vm.Remark = model.Remark;
                return View("PondEdit", vm);
            }

            await _service.CreatePondAsync(model, _current.Name);
            TempData["SuccessMessage"] = "新增養殖池成功";
            return RedirectToAction(nameof(PondList), new { areaSN = model.AreaSN });
        }

        [HttpGet]
        public async Task<IActionResult> EditPond(int id)
        {
            var pond = await _service.GetPondEditViewModelAsync(id, 0); // service 會自己找 areaSN
            return View("PondEdit", pond);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPond(PondEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var vm = await _service.GetPondEditViewModelAsync(model.PondSN, model.AreaSN);
                vm.PondNum = model.PondNum;
                vm.PondWidth = model.PondWidth;
                vm.PondLength = model.PondLength;
                vm.PondArea = model.PondArea;
                vm.GPSX = model.GPSX;
                vm.GPSY = model.GPSY;
                vm.Remark = model.Remark;
                return View("PondEdit", vm);
            }

            await _service.UpdatePondAsync(model, _current.Name);
            TempData["SuccessMessage"] = "修改養殖池成功";
            return RedirectToAction(nameof(PondList), new { areaSN = model.AreaSN });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePond(int id, int areaSN)
        {
            await _service.DeletePondAsync(id, _current.Name);
            TempData["SuccessMessage"] = "刪除養殖池成功";
            return RedirectToAction(nameof(PondList), new { areaSN });
        }
    }
}
