using ADAMS.Areas.BasicDataManagement.Repositories.FeedDataManagement;
using ADAMS.Areas.BasicDataManagement.ViewModels.FeedDataManagement;
using ADAMS.Areas.Models;
using ADAMS.Models;
using ADAMS.Services;

namespace ADAMS.Areas.BasicDataManagement.Services.FeedDataManagement
{
    public class FeedDataService : IFeedDataService
    {
        private readonly IFeedDataRepository _repo;
        private readonly ICurrentAccountService _current;

        public FeedDataService(
            IFeedDataRepository repo,
            ICurrentAccountService current)
        {
            _repo = repo;
            _current = current;
        }

        private (bool isOperationCompany, int tenantSN) GetTenantContext(int? fromQuery)
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

        public async Task<FeedListViewModel> GetListViewModelAsync(
            int? tenantSN,
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

            var (data, totalCount) =
                await _repo.GetPagedListAsync(currentTenantSN, keyword, page, pageSize);

            var items = data.Select(f => new FeedListItemViewModel
            {
                FeedSN = f.FeedSN,
                FeedName = f.FeedName,
                SupplierName = f.Supplier?.SupplierName ?? string.Empty,
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
                    { "tenantSN", currentTenantSN },
                    { "keyword", keyword ?? string.Empty }
                }
            };

            return new FeedListViewModel
            {
                IsOperationCompany = isOp,
                CurrentTenantSN = currentTenantSN,
                TenantOptions = tenantOptions,
                Keyword = keyword,
                Feeds = items,
                Pagination = pagination
            };
        }

        public async Task<FeedEditViewModel> GetCreateViewModelAsync()
        {
            var (isOp, currentTenantSN) = GetTenantContext(null);

            var tenantOptions = await _repo.GetTenantOptionsAsync(isOp, currentTenantSN);
            if ((currentTenantSN == 0 || !tenantOptions.Any(t => t.SN == currentTenantSN))
                && tenantOptions.Any())
            {
                currentTenantSN = tenantOptions.First().SN;
            }

            var suppliers = await _repo.GetSuppliersForTenantAsync(currentTenantSN);

            return new FeedEditViewModel
            {
                IsOperationCompany = isOp,
                TenantSN = currentTenantSN,
                TenantName = tenantOptions.FirstOrDefault(t => t.SN == currentTenantSN)?.TenantName ?? "",
                TenantOptions = tenantOptions,
                SupplierOptions = suppliers.Select(s => new SupplierOption
                {
                    SupplierSN = s.SupplierSN,
                    SupplierName = s.SupplierName
                }).ToList(),
                UnitName = "公斤"
            };
        }

        public async Task<FeedEditViewModel?> GetEditViewModelAsync(int feedSN)
        {
            var feed = await _repo.GetAsync(feedSN);
            if (feed == null) return null;

            bool isOp = _current.IsOperationCompany;

            var tenantOptions = await _repo.GetTenantOptionsAsync(isOp, feed.TenantSN);
            var suppliers = await _repo.GetSuppliersForTenantAsync(feed.TenantSN);

            return new FeedEditViewModel
            {
                FeedSN = feed.FeedSN,
                TenantSN = feed.TenantSN,
                TenantName = tenantOptions.FirstOrDefault(t => t.SN == feed.TenantSN)?.TenantName ?? "",
                IsOperationCompany = isOp,
                TenantOptions = tenantOptions,
                SupplierSN = feed.SupplierSN,
                SupplierOptions = suppliers.Select(s => new SupplierOption
                {
                    SupplierSN = s.SupplierSN,
                    SupplierName = s.SupplierName
                }).ToList(),
                FeedName = feed.FeedName,
                UnitName = feed.UnitName,
                Remark = feed.Remark
            };
        }

        public async Task<bool> CreateAsync(FeedEditViewModel model)
        {
            var entity = new Feed
            {
                TenantSN = model.TenantSN,
                SupplierSN = model.SupplierSN,
                FeedName = model.FeedName,
                UnitName = string.IsNullOrWhiteSpace(model.UnitName) ? "公斤" : model.UnitName,
                Remark = model.Remark,
                IsDeleted = false,
                CreateTime = DateTime.Now,
                CreateUser = _current.Name
            };

            await _repo.CreateAsync(entity);
            return true;
        }

        public async Task<bool> UpdateAsync(FeedEditViewModel model)
        {
            var entity = await _repo.GetAsync(model.FeedSN!.Value);
            if (entity == null) return false;

            entity.TenantSN = model.TenantSN;
            entity.SupplierSN = model.SupplierSN;
            entity.FeedName = model.FeedName;
            entity.UnitName = string.IsNullOrWhiteSpace(model.UnitName) ? "公斤" : model.UnitName;
            entity.Remark = model.Remark;
            entity.ModifyTime = DateTime.Now;
            entity.ModifyUser = _current.Name;

            await _repo.UpdateAsync(entity);
            return true;
        }

        public async Task<bool> SoftDeleteAsync(int feedSN)
        {
            await _repo.SoftDeleteAsync(feedSN, _current.Name);
            return true;
        }
    }
}
