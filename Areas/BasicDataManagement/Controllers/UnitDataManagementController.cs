using System;
using ADAMS.Areas.BasicDataManagement.Services.UnitDataManagement;
using ADAMS.Areas.BasicDataManagement.ViewModels.UnitDataManagement;
using ADAMS.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace ADAMS.Areas.BasicDataManagement.Controllers
{
    [Area("BasicDataManagement")]
    [FunctionPermission(15)]
    public class UnitDataManagementController : Controller
    {
        private readonly IUnitDataService _service;

        public UnitDataManagementController(IUnitDataService service)
        {
            _service = service;
        }

        // GET: /BasicDataManagement/UnitDataManagement
        [HttpGet]
        public async Task<IActionResult> Index(string? keyword)
        {
            var vm = await _service.GetListViewModelAsync(keyword);
            return View(vm);
        }

        // GET: Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = await _service.GetCreateViewModelAsync();
            if (!vm.IsOperationCompany)
            {
                TempData["ErrorMessage"] = "僅營運公司帳號可維護單位資料。";
                return RedirectToAction(nameof(Index));
            }

            return View("Edit", vm);
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UnitDataEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.IsOperationCompany = true;
                return View("Edit", model);
            }

            try
            {
                await _service.CreateAsync(model);
                TempData["SuccessMessage"] = "新增單位成功。";
                return RedirectToAction(nameof(Index), new { keyword = (string?)null });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                model.IsOperationCompany = true;
                return View("Edit", model);
            }
        }

        // GET: Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var vm = await _service.GetEditViewModelAsync(id);
            if (vm == null)
            {
                TempData["ErrorMessage"] = "找不到指定的單位資料。";
                return RedirectToAction(nameof(Index));
            }

            if (!vm.IsOperationCompany)
            {
                TempData["ErrorMessage"] = "僅營運公司帳號可維護單位資料。";
                return RedirectToAction(nameof(Index));
            }

            return View(vm);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UnitDataEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.IsOperationCompany = true;
                return View(model);
            }

            try
            {
                await _service.UpdateAsync(model);
                TempData["SuccessMessage"] = "更新單位成功。";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                model.IsOperationCompany = true;
                return View(model);
            }
        }

        // POST: Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.SoftDeleteAsync(id);
                TempData["SuccessMessage"] = "刪除單位成功。";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
