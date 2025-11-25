using ADAMS.Areas.BasicDataManagement.Services.TimeSegmentSetting;
using ADAMS.Areas.BasicDataManagement.ViewModels.TimeSegmentSetting;
using ADAMS.Attributes;
using ADAMS.Services;
using Microsoft.AspNetCore.Mvc;

namespace ADAMS.Areas.BasicDataManagement.Controllers
{
    [Area("BasicDataManagement")]
    [FunctionPermission(11)] //TimeSegmentSetting

    public class TimeSegmentSettingController : Controller
    {
        private readonly ITimeSegmentService _service;
        private readonly ILogger<TimeSegmentSettingController> _logger;
        private readonly ICurrentAccountService _current;

        public TimeSegmentSettingController(
            ITimeSegmentService service,
            ILogger<TimeSegmentSettingController> logger,
            ICurrentAccountService current)
        {
            _service = service;
            _logger = logger;
            _current = current;
        }

        public async Task<IActionResult> Index(
            int? tenantSN,
            string? keyword,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                var vm = await _service.GetListViewModelAsync(
                    tenantSN, keyword, page, pageSize);
                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "載入時間區段列表失敗");
                TempData["ErrorMessage"] = "載入資料失敗，請稍後再試";
                return View(new TimeZoneListViewModel());
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
        public async Task<IActionResult> Create(TimeZoneEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // 重新取得下拉資料
                var vm = await _service.GetCreateViewModelAsync(model.TenantSN);
                vm.TimeZoneNum = model.TimeZoneNum;
                vm.TimeZoneDesc = model.TimeZoneDesc;
                vm.Remark = model.Remark;

                return View("Edit", vm);
            }

            try
            {
                var currentUser = _current.Name ?? "system";
                var (success, message) = await _service.CreateAsync(model, currentUser);

                if (!success)
                {
                    TempData["ErrorMessage"] = message;
                    var vm = await _service.GetCreateViewModelAsync(model.TenantSN);
                    vm.TimeZoneNum = model.TimeZoneNum;
                    vm.TimeZoneDesc = model.TimeZoneDesc;
                    vm.Remark = model.Remark;
                    return View("Edit", vm);
                }

                TempData["SuccessMessage"] = message;
                return RedirectToAction(nameof(Index), new { tenantSN = model.TenantSN });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "新增時間區段失敗");
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
                TempData["ErrorMessage"] = "找不到時間區段資料。";
                return RedirectToAction(nameof(Index));
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TimeZoneEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // 編輯時 TenantOptions 仍然需要
                var vm = await _service.GetEditViewModelAsync(model.TimeZoneSN!.Value);
                if (vm != null)
                {
                    vm.TimeZoneNum = model.TimeZoneNum;
                    vm.TimeZoneDesc = model.TimeZoneDesc;
                    vm.Remark = model.Remark;
                    return View(vm);
                }

                return View(model);
            }

            try
            {
                var currentUser = _current.Name ?? "system";
                var (success, message) = await _service.UpdateAsync(model, currentUser);

                if (!success)
                {
                    TempData["ErrorMessage"] = message;
                    var vm = await _service.GetEditViewModelAsync(model.TimeZoneSN!.Value);
                    if (vm != null)
                    {
                        vm.TimeZoneNum = model.TimeZoneNum;
                        vm.TimeZoneDesc = model.TimeZoneDesc;
                        vm.Remark = model.Remark;
                        return View(vm);
                    }
                    return View(model);
                }

                TempData["SuccessMessage"] = message;
                return RedirectToAction(nameof(Index), new { tenantSN = model.TenantSN });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新時間區段失敗");
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

                TempData["SuccessMessage"] = "時間區段已刪除。";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除時間區段失敗 TimeZoneSN={Id}", id);
                TempData["ErrorMessage"] = "刪除失敗，請稍後再試";
            }

            return RedirectToAction(nameof(Index), new { tenantSN });
        }
    }
}

