using ADAMS.Areas.BasicDataManagement.ViewModels.SupplierDataManagement;

namespace ADAMS.Areas.BasicDataManagement.Services.SupplierDataManagement
{
    public interface ISupplierDataService
    {
        Task<SupplierListViewModel> GetListViewModelAsync(
            int? tenantSN,
            string? keyword,
            int supplierType,
            int page,
            int pageSize);

        Task<SupplierEditViewModel> GetCreateViewModelAsync(int? tenantSNFromQuery = null);

        Task<SupplierEditViewModel?> GetEditViewModelAsync(int supplierSN);

        Task CreateAsync(SupplierEditViewModel model, string currentUser);

        Task UpdateAsync(SupplierEditViewModel model, string currentUser);

        Task SoftDeleteAsync(int supplierSN, string currentUser);
    }
}
