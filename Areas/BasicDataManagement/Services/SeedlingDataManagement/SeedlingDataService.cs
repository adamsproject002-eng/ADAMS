using ADAMS.Areas.BasicDataManagement.Repositories.SeedlingDataManagement;
using ADAMS.Areas.BasicDataManagement.ViewModels.SeedlingDataManagement;
using ADAMS.Areas.Models;
using ADAMS.Models;
using ADAMS.Services;

namespace ADAMS.Areas.BasicDataManagement.Services.SeedlingDataManagement
{
    public class SeedlingDataService : ISeedlingDataService
    {
        private readonly ISeedlingDataRepository _repo;
        private readonly ICurrentAccountService _current;

        public SeedlingDataService(ISeedlingDataRepository repo, ICurrentAccountService current)
        {
            _repo = repo;
            _current = current;
        }

        private (bool isOp, int tenantSN) GetTenantContext(int? fromQuery)
        {
            if (_current.IsOperationCompany)
            {
                var tenantSN = fromQuery ?? _current.TenantSN;
                return (true, tenantSN);
            }
            else
            {
                return (false, _current.TenantSN);
            }
        }

        public async Task<SeedlingListViewModel> GetListViewModelAsync(
            int? tenantSN,
            int selectedFVSN,
            string? keyword,
            int page,
            int pageSize)
        {
            var (isOp, currentTenantSN) = GetTenantContext(tenantSN);

            var tenantOptions = await _repo.GetTenantOptionsAsync(isOp, currentTenantSN);

            if ((currentTenantSN == 0 || !tenantOptions.Any(t => t.SN == currentTenantSN))
                && tenantOptions.Any())
            {
                currentTenantSN = tenantOptions.First().SN;
            }

            var varietyOptions = await _repo.GetFishVarietyOptionsAsync();

            var (data, totalCount) = await _repo.GetPagedListAsync(
                currentTenantSN,
                selectedFVSN,
                keyword,
                page,
                pageSize);

            var items = data.Select(f => new SeedlingListItemViewModel
            {
                FrySN = f.FrySN,
                FryName = f.FryName,
                SupplierName = f.Supplier?.SupplierName ?? "",
                FishVarietyName = f.FishVariety?.FVName ?? "",
                UnitName = f.UnitName,
                Remark = f.Remark
            }).ToList();

            var pagination = new PaginationInfo
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                SearchFilters = new Dictionary<string, object>
                {
                    ["tenantSN"] = currentTenantSN,
                    ["selectedFVSN"] = selectedFVSN,
                    ["keyword"] = keyword ?? ""
                }
            };

            return new SeedlingListViewModel
            {
                IsOperationCompany = isOp,
                CurrentTenantSN = currentTenantSN,
                TenantOptions = tenantOptions,
                FishVarietyOptions = varietyOptions,
                SelectedFVSN = selectedFVSN,
                Keyword = keyword,
                Items = items,
                Pagination = pagination
            };
        }

        public async Task<SeedlingEditViewModel> GetCreateViewModelAsync()
        {
            var (isOp, currentTenantSN) = GetTenantContext(null);

            var tenantOptions = await _repo.GetTenantOptionsAsync(isOp, currentTenantSN);
            if ((currentTenantSN == 0 || !tenantOptions.Any(t => t.SN == currentTenantSN))
                && tenantOptions.Any())
            {
                currentTenantSN = tenantOptions.First().SN;
            }

            var supplierOptions = await _repo.GetSupplierOptionsAsync(currentTenantSN);
            var varietyOptions = await _repo.GetFishVarietyOptionsAsync();

            var currentTenantName = tenantOptions
                .FirstOrDefault(t => t.SN == currentTenantSN)?.TenantName ?? "";

            return new SeedlingEditViewModel
            {
                TenantSN = currentTenantSN,
                TenantName = currentTenantName,
                IsOperationCompany = isOp,
                TenantOptions = tenantOptions,
                SupplierOptions = supplierOptions,
                FishVarietyOptions = varietyOptions
            };
        }

        public async Task<SeedlingEditViewModel?> GetEditViewModelAsync(int frySN)
        {
            var entity = await _repo.GetAsync(frySN);
            if (entity == null) return null;

            var (isOp, _) = GetTenantContext(entity.TenantSN);

            var tenantOptions = await _repo.GetTenantOptionsAsync(isOp, entity.TenantSN);
            var supplierOptions = await _repo.GetSupplierOptionsAsync(entity.TenantSN);
            var varietyOptions = await _repo.GetFishVarietyOptionsAsync();

            var tenantName = tenantOptions.FirstOrDefault(t => t.SN == entity.TenantSN)?.TenantName ?? "";

            return new SeedlingEditViewModel
            {
                FrySN = entity.FrySN,
                TenantSN = entity.TenantSN,
                TenantName = tenantName,
                FryName = entity.FryName,
                SupplierSN = entity.SupplierSN,
                FVSN = entity.FVSN,
                UnitName = entity.UnitName,
                Remark = entity.Remark,
                IsOperationCompany = isOp,
                TenantOptions = tenantOptions,
                SupplierOptions = supplierOptions,
                FishVarietyOptions = varietyOptions
            };
        }

        public async Task CreateAsync(SeedlingEditViewModel model)
        {
            var entity = new Fry
            {
                TenantSN = model.TenantSN,
                FryName = model.FryName,
                SupplierSN = model.SupplierSN,
                FVSN = model.FVSN,
                UnitName = model.UnitName,
                Remark = model.Remark,
                IsDeleted = false,
                CreateTime = DateTime.Now,
                CreateUser = _current.Name
            };

            await _repo.AddAsync(entity);
        }

        public async Task UpdateAsync(SeedlingEditViewModel model)
        {
            if (!model.FrySN.HasValue) throw new InvalidOperationException("缺少 FrySN");

            var entity = await _repo.GetAsync(model.FrySN.Value);
            if (entity == null) throw new InvalidOperationException("找不到魚苗資料");

            entity.FryName = model.FryName;
            entity.SupplierSN = model.SupplierSN;
            entity.FVSN = model.FVSN;
            entity.UnitName = model.UnitName;
            entity.Remark = model.Remark;
            entity.ModifyTime = DateTime.Now;
            entity.ModifyUser = _current.Name;

            await _repo.UpdateAsync(entity);
        }

        public Task DeleteAsync(int frySN)
        {
            return _repo.SoftDeleteAsync(frySN, _current.Name);
        }
    }
}
