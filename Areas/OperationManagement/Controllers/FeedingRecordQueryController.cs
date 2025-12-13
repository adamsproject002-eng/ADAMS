using ADAMS.Areas.OperationManagement.Services.FeedingRecordQuery;
using ADAMS.Areas.OperationManagement.ViewModels.FeedingRecordQuery;
using ADAMS.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace ADAMS.Areas.OperationManagement.Controllers
{
    [Area("OperationManagement")]
    [FunctionPermission(22)]
    public class FeedingRecordQueryController : Controller
    {
        private readonly IFeedingRecordService _service;

        public FeedingRecordQueryController(IFeedingRecordService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            int? tenantSN,
            int? areaSN,
            DateTime? startDate,
            DateTime? endDate)
        {
            var vm = await _service.GetListViewModelAsync(tenantSN, areaSN, startDate, endDate);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? tenantSN, int? areaSN, int? pondSN)
        {
            var vm = await _service.GetCreateViewModelAsync(tenantSN, areaSN, pondSN);
            return View("Edit", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FeedingRecordEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // 回填下拉
                var vm = await _service.GetCreateViewModelAsync(model.TenantSN, model.AreaSN, model.PondSN);
                vm.FeedingAmount = model.FeedingAmount;
                vm.SurvivalRate = model.SurvivalRate;
                vm.ABW = model.ABW;
                vm.DOC = model.DOC;
                vm.ManageAccount = model.ManageAccount;
                return View("Edit", vm);
            }

            await _service.CreateAsync(model);
            TempData["SuccessMessage"] = "新增投餌紀錄成功。";
            return RedirectToAction(nameof(Index), new { tenantSN = model.TenantSN, areaSN = model.AreaSN });
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
        public async Task<IActionResult> Edit(FeedingRecordEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // 回填下拉
                var vm = await _service.GetEditViewModelAsync(model.FeedingRecordSN!.Value);
                if (vm == null) return NotFound();
                return View(vm);
            }

            await _service.UpdateAsync(model);
            TempData["SuccessMessage"] = "更新投餌紀錄成功。";
            return RedirectToAction(nameof(Index), new { tenantSN = model.TenantSN, areaSN = model.AreaSN });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int tenantSN, int? areaSN)
        {
            await _service.SoftDeleteAsync(id);
            TempData["SuccessMessage"] = "已刪除投餌紀錄。";
            return RedirectToAction(nameof(Index), new { tenantSN, areaSN });
        }

        [HttpGet]
        public async Task<IActionResult> Export(
            int? tenantSN,
            int? areaSN,
            DateTime? startDate,
            DateTime? endDate)
        {
            var bytes = await _service.ExportCsvAsync(tenantSN, areaSN, startDate, endDate);
            var fileName = $"FeedingRecord_{DateTime.Now:yyyyMMddHHmmss}.csv";
            return File(bytes, "text/csv", fileName);
        }

        /// <summary>
        /// 給前端 AJAX 用：依養殖戶 + 場區取得池號清單
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPonds(int tenantSN, int areaSN)
        {
            var ponds = await _service.GetPondOptionsAsync(tenantSN, areaSN);
            // 回傳像 [{ "pondSN":1,"pondNum":"A01" }, ...]
            return Json(ponds);
        }
        /// <summary>
        /// 依供應商動態載入飼料選項 (給 JS 用, 回傳 JSON)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetFeeds(int tenantSN, int supplierSN)
        {
            var feeds = await _service.GetFeedOptionsAsync(tenantSN, supplierSN);

            var result = feeds.Select(f => new
            {
                feedSN = f.FeedSN,
                feedName = f.FeedName,
                unitName = f.UnitName
            });

            return Json(result);
        }
    }
}
