using ADAMS.Areas.BasicDataManagement.Repositories.FeedingPlanManagement;
using ADAMS.Areas.BasicDataManagement.ViewModels.FeedingPlanManagement;
using ADAMS.Models;
using ADAMS.Services;

namespace ADAMS.Areas.BasicDataManagement.Services.FeedingPlanManagement
{
    public class FeedingPlanService : IFeedingPlanService
    {
        private readonly IFeedingPlanRepository _repo;
        private readonly ICurrentAccountService _current;

        public FeedingPlanService(
            IFeedingPlanRepository repo,
            ICurrentAccountService current)
        {
            _repo = repo;
            _current = current;
        }

        public async Task<FeedingPlanListViewModel> GetListViewModelAsync(int? tenantSN, int? fvsn)
        {
            var isOp = _current.IsOperationCompany;
            var currentTenantSN = isOp ? (tenantSN ?? _current.TenantSN) : _current.TenantSN;

            // 養殖戶選項
            var tenants = await _repo.GetTenantOptionsAsync(isOp, _current.TenantSN);
            if (!tenants.Any())
            {
                return new FeedingPlanListViewModel
                {
                    IsOperationCompany = isOp
                };
            }

            if (!tenants.Any(t => t.SN == currentTenantSN))
            {
                currentTenantSN = tenants.First().SN;
            }

            var tenant = tenants.First(t => t.SN == currentTenantSN);

            // 魚種選項
            var varieties = await _repo.GetFishVarietiesAsync();
            if (!varieties.Any())
            {
                return new FeedingPlanListViewModel
                {
                    IsOperationCompany = isOp,
                    CurrentTenantSN = currentTenantSN,
                    CurrentTenantName = tenant.TenantName
                };
            }

            var selectedFvsn = fvsn ?? varieties.First().FVSN;
            var selectedVariety = varieties.FirstOrDefault(v => v.FVSN == selectedFvsn) ?? varieties.First();

            // 主表＋明細
            var main = await _repo.GetMainAsync(currentTenantSN, selectedVariety.FVSN);
            var items = new List<FeedingPlanListItemViewModel>();

            if (main != null)
            {
                var details = await _repo.GetDetailsAsync(main.FPMSN);
                items = details.Select(d => new FeedingPlanListItemViewModel
                {
                    FPDSN = d.FPDSN,
                    DOC = d.FP_DOC,
                    ABW = d.FP_ABW,
                    FeedingRate = d.FP_FeedingRate,
                    Remark = d.Remark
                }).ToList();
            }

            return new FeedingPlanListViewModel
            {
                IsOperationCompany = isOp,
                CurrentTenantSN = currentTenantSN,
                CurrentTenantName = tenant.TenantName,
                SelectedFVSN = selectedVariety.FVSN,
                SelectedFishVarietyName = selectedVariety.FVName,
                TenantOptions = tenants.Select(t => new TenantOption { SN = t.SN, TenantName = t.TenantName }).ToList(),
                FishVarietyOptions = varieties.Select(v => new FishVarietyOption { FVSN = v.FVSN, FVName = v.FVName }).ToList(),
                Items = items
            };
        }

        public async Task<FeedingPlanEditViewModel> GetCreateViewModelAsync(int? tenantSN, int? fvsn)
        {
            var listVm = await GetListViewModelAsync(tenantSN, fvsn);

            return new FeedingPlanEditViewModel
            {
                FPDSN = null,
                FPMSN = 0, // 建立時再決定
                TenantSN = listVm.CurrentTenantSN,
                TenantName = listVm.CurrentTenantName,
                FVSN = listVm.SelectedFVSN,
                FishVarietyName = listVm.SelectedFishVarietyName,
                IsOperationCompany = listVm.IsOperationCompany
            };
        }

        public async Task<FeedingPlanEditViewModel?> GetEditViewModelAsync(int fpdsn)
        {
            var detail = await _repo.GetDetailAsync(fpdsn);
            if (detail == null || detail.Main == null || detail.Main.FishVariety == null || detail.Main.Tenant == null)
                return null;

            var main = detail.Main;
            var tenant = main.Tenant;
            var fv = main.FishVariety;

            return new FeedingPlanEditViewModel
            {
                FPDSN = detail.FPDSN,
                FPMSN = main.FPMSN,
                TenantSN = tenant.SN,
                TenantName = tenant.TenantName,
                FVSN = fv.FVSN,
                FishVarietyName = fv.FVName,
                DOC = detail.FP_DOC,
                ABW = detail.FP_ABW,
                FeedingRate = detail.FP_FeedingRate,
                Remark = detail.Remark,
                IsOperationCompany = _current.IsOperationCompany
            };
        }

        public async Task CreateAsync(FeedingPlanEditViewModel model)
        {
            // 任何養殖戶（含營運公司代管）都可以維護自己的規劃，所以不限制 IsOperationCompany

            // 先取得主表，沒有就建
            var main = await _repo.GetMainAsync(model.TenantSN, model.FVSN);
            if (main == null)
            {
                var planName = $"{model.FishVarietyName}投餌規劃";
                main = await _repo.CreateMainAsync(model.TenantSN, model.FVSN, planName, _current.Name);
            }

            var detail = new FeedingPlanDetail
            {
                FPMSN = main.FPMSN,
                FP_DOC = model.DOC,
                FP_ABW = model.ABW,
                FP_FeedingRate = model.FeedingRate,
                Remark = model.Remark,
                IsDeleted = false,
                CreateTime = DateTime.Now,
                CreateUser = _current.Name
            };

            await _repo.AddDetailAsync(detail);
        }

        public async Task UpdateAsync(FeedingPlanEditViewModel model)
        {
            var entity = await _repo.GetDetailAsync(model.FPDSN!.Value);
            if (entity == null) return;

            entity.FP_DOC = model.DOC;
            entity.FP_ABW = model.ABW;
            entity.FP_FeedingRate = model.FeedingRate;
            entity.Remark = model.Remark;
            entity.ModifyTime = DateTime.Now;
            entity.ModifyUser = _current.Name;

            await _repo.UpdateDetailAsync(entity);
        }

        public async Task SoftDeleteAsync(int fpdsn)
        {
            await _repo.SoftDeleteDetailAsync(fpdsn, _current.Name);
        }
    }
}
