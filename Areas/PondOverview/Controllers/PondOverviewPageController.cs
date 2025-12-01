using ADAMS.Areas.PondOverview.Services.PondOverviewPage;
using ADAMS.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace ADAMS.Areas.PondOverview.Controllers
{
    [Area("PondOverview")]
    [FunctionPermission(16)] //養殖池總覽(首頁)
    public class PondOverviewPageController : Controller
    {
        private readonly IPondOverviewService _service;

        public PondOverviewPageController(IPondOverviewService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? tenantSN, int? areaSN)
        {
            var vm = await _service.GetOverviewAsync(tenantSN, areaSN);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var vm = await _service.GetDetailViewModelAsync(id);
            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);
        }
    }
}
