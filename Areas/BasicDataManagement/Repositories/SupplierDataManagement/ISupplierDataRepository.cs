using ADAMS.Areas.Models;
using ADAMS.Models;
using ADAMS.Areas.BasicDataManagement.ViewModels.SupplierDataManagement;

namespace ADAMS.Areas.BasicDataManagement.Repositories.SupplierDataManagement
{
    public interface ISupplierDataRepository
    {
        Task<(List<Supplier> Data, int TotalCount)> GetPagedListAsync(
            int tenantSN,
            string? keyword,
            int supplierType,
            int page,
            int pageSize);

        Task<List<TenantOption>> GetTenantOptionsAsync(
            bool isOperationCompany,
            int currentTenantSN);

        Task<Supplier?> GetByIdAsync(int supplierSN);

        Task AddAsync(Supplier supplier);
        Task UpdateAsync(Supplier supplier);
        Task SoftDeleteAsync(int supplierSN, string currentUser);
    }
}
