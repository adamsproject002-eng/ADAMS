using ADAMS.Areas.BasicDataManagement.Repositories.SupplierDataManagement;
using ADAMS.Areas.BasicDataManagement.ViewModels.SupplierDataManagement;
using ADAMS.Areas.Models;
using ADAMS.Models;
using ADAMS.Services;

namespace ADAMS.Areas.BasicDataManagement.Services.SupplierDataManagement
{
    public class SupplierDataService : ISupplierDataService
    {
        private readonly ISupplierDataRepository _repo;
        private readonly ICurrentAccountService _current;

        public SupplierDataService(
            ISupplierDataRepository repo,
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

            return (false, _current.TenantSN);
        }

        private static string GetSupplierTypeName(int type)
        {
            return type switch
            {
                1 => "飼料",
                2 => "魚苗",
                3 => "其他",
                _ => ""
            };
        }

        public async Task<SupplierListViewModel> GetListViewModelAsync(
            int? tenantSN,
            string? keyword,
            int supplierType,
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

            var (data, totalCount) = await _repo.GetPagedListAsync(
                currentTenantSN, keyword, supplierType, page, pageSize);

            var items = data.Select(s => new SupplierListItemViewModel
            {
                SupplierSN = s.SupplierSN,
                SupplierNum = s.SupplierNum,
                SupplierName = s.SupplierName,
                ContactName = s.ContactName,
                ContactPhone = s.ContactPhone,
                SupplierType = s.SupplierType,
                SupplierTypeName = GetSupplierTypeName(s.SupplierType),
                Remark = s.Remark
            }).ToList();

            var vm = new SupplierListViewModel
            {
                IsOperationCompany = isOp,
                CurrentTenantSN = currentTenantSN,
                TenantOptions = tenantOptions,
                Keyword = keyword,
                SupplierType = supplierType,
                Suppliers = items,
                Pagination = new PaginationInfo
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    SearchFilters = new Dictionary<string, object>
                    {
                        { "tenantSN", tenantSN ?? 1 },
                        { "keyword", keyword ?? "" },
                        { "supplierType" , supplierType}
                    }
                }
            };

            return vm;
        }

        public async Task<SupplierEditViewModel> GetCreateViewModelAsync(int? tenantSNFromQuery = null)
        {
            var (isOp, currentTenantSN) = GetTenantContext(tenantSNFromQuery);

            var tenantOptions = await _repo.GetTenantOptionsAsync(isOp, currentTenantSN);

            if ((currentTenantSN == 0 || !tenantOptions.Any(t => t.SN == currentTenantSN))
                && tenantOptions.Any())
            {
                currentTenantSN = tenantOptions.First().SN;
            }

            var currentTenant = tenantOptions.FirstOrDefault(t => t.SN == currentTenantSN);
            var tenantName = currentTenant?.TenantName ?? string.Empty;

            return new SupplierEditViewModel
            {
                SupplierSN = null,
                TenantSN = currentTenantSN,
                TenantName = tenantName,
                SupplierType = 1, // 預設飼料
                IsOperationCompany = isOp,
                TenantOptions = tenantOptions
            };
        }

        public async Task<SupplierEditViewModel?> GetEditViewModelAsync(int supplierSN)
        {
            var supplier = await _repo.GetByIdAsync(supplierSN);
            if (supplier == null)
                return null;

            var (isOp, _) = GetTenantContext(supplier.TenantSN);
            var tenantOptions = await _repo.GetTenantOptionsAsync(isOp, supplier.TenantSN);
            var currentTenant = tenantOptions.FirstOrDefault(t => t.SN == supplier.TenantSN);
            var tenantName = currentTenant?.TenantName ?? string.Empty;

            return new SupplierEditViewModel
            {
                SupplierSN = supplier.SupplierSN,
                TenantSN = supplier.TenantSN,
                TenantName = tenantName,
                SupplierNum = supplier.SupplierNum,
                SupplierName = supplier.SupplierName,
                ContactName = supplier.ContactName,
                ContactPhone = supplier.ContactPhone,
                SupplierType = supplier.SupplierType,
                Remark = supplier.Remark,
                IsOperationCompany = isOp,
                TenantOptions = tenantOptions
            };
        }

        public async Task CreateAsync(SupplierEditViewModel model, string currentUser)
        {
            var entity = new Supplier
            {
                TenantSN = model.TenantSN,
                SupplierNum = model.SupplierNum,
                SupplierName = model.SupplierName,
                ContactName = model.ContactName,
                ContactPhone = model.ContactPhone,
                SupplierType = model.SupplierType,
                Remark = model.Remark,
                IsDeleted = false,
                CreateTime = DateTime.Now,
                CreateUser = currentUser
            };

            await _repo.AddAsync(entity);
        }

        public async Task UpdateAsync(SupplierEditViewModel model, string currentUser)
        {
            var entity = await _repo.GetByIdAsync(model.SupplierSN!.Value);
            if (entity == null)
                throw new InvalidOperationException("找不到指定的供應商資料。");

            entity.SupplierNum = model.SupplierNum;
            entity.SupplierName = model.SupplierName;
            entity.ContactName = model.ContactName;
            entity.ContactPhone = model.ContactPhone;
            entity.SupplierType = model.SupplierType;
            entity.Remark = model.Remark;
            entity.ModifyTime = DateTime.Now;
            entity.ModifyUser = currentUser;

            await _repo.UpdateAsync(entity);
        }

        public async Task SoftDeleteAsync(int supplierSN, string currentUser)
        {
            await _repo.SoftDeleteAsync(supplierSN, currentUser);
        }
    }
}
