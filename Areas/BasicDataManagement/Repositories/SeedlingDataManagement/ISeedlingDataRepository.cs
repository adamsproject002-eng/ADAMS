using ADAMS.Areas.Models;
using ADAMS.Areas.BasicDataManagement.ViewModels.SeedlingDataManagement;
using ADAMS.Models;

namespace ADAMS.Areas.BasicDataManagement.Repositories.SeedlingDataManagement
{
    public interface ISeedlingDataRepository
    {
        Task<(List<Fry> Data, int TotalCount)> GetPagedListAsync(
            int tenantSN,
            int selectedFVSN,
            string? keyword,
            int page,
            int pageSize);

        Task<List<TenantOption>> GetTenantOptionsAsync(bool isOperationCompany, int currentTenantSN);

        Task<List<FishVarietyOption>> GetFishVarietyOptionsAsync();

        Task<List<SupplierOption>> GetSupplierOptionsAsync(int tenantSN);

        Task<Fry?> GetAsync(int frySN);

        Task AddAsync(Fry entity);

        Task UpdateAsync(Fry entity);

        Task SoftDeleteAsync(int frySN, string currentUser);
    }
}
