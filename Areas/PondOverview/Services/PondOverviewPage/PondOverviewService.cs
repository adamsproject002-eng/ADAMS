using ADAMS.Areas.PondOverview.Repositories.PondOverviewPage;
using ADAMS.Areas.PondOverview.ViewModels.PondOverviewPage;
using ADAMS.Services;

namespace ADAMS.Areas.PondOverview.Services.PondOverviewPage
{
    public class PondOverviewService : IPondOverviewService
    {
        private readonly IPondOverviewRepository _repo;
        private readonly ICurrentAccountService _current;

        public PondOverviewService(
            IPondOverviewRepository repo,
            ICurrentAccountService current)
        {
            _repo = repo;
            _current = current;
        }

        public async Task<PondOverviewListViewModel> GetOverviewAsync(int? tenantSN, int? areaSN)
        {
            var isOp = _current.IsOperationCompany;

            // 1. 決定目前 TenantSN
            var currentTenantSN = isOp
                ? (tenantSN ?? _current.TenantSN)
                : _current.TenantSN;

            var tenantOptions = await _repo.GetTenantOptionsAsync(isOp, currentTenantSN);

            if (isOp && tenantOptions.Any() && !tenantOptions.Any(t => t.SN == currentTenantSN))
            {
                currentTenantSN = tenantOptions.First().SN;
            }

            // 2. 場區下拉
            var areaOptions = await _repo.GetAreaOptionsAsync(currentTenantSN);
            int selectedAreaSN;
            //如果下拉的場區不是屬於該養殖戶，則預設選第一個場區
            if (areaSN.HasValue && areaOptions.Any(a => a.AreaSN == areaSN.Value))
            {
                selectedAreaSN = areaSN.Value;
            }
            else
            {
                selectedAreaSN = areaOptions.FirstOrDefault()?.AreaSN ?? 0;
            }

            var ponds = await _repo.GetPondCardsAsync(currentTenantSN, selectedAreaSN);

            var areaName = areaOptions
                .FirstOrDefault(a => a.AreaSN == selectedAreaSN)?.AreaName ?? string.Empty;

            var totalArea = ponds.Sum(p => p.PondArea);

            return new PondOverviewListViewModel
            {
                IsOperationCompany = isOp,
                CurrentTenantSN = currentTenantSN,
                TenantOptions = tenantOptions,
                CurrentAreaSN = selectedAreaSN,
                AreaOptions = areaOptions,
                CurrentAreaName = areaName,
                CurrentAreaTotalArea = totalArea,
                Ponds = ponds
            };
        }

        public async Task<PondDetailViewModel?> GetDetailViewModelAsync(int pondSN)
        {
            // 若需要檢查 TenantSN 權限，可以在這裡補一層驗證
            return await _repo.GetPondDetailAsync(pondSN);
        }
    }
}
