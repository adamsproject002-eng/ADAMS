using ADAMS.Models;

namespace ADAMS.Areas.AccountPermissionManagement.Repositories.FarmerManagement
{
    public interface IFarmerRepository
    {
        Task<(List<Tenant> Data, int TotalCount)> GetPagedListAsync(
            string? statusFilter,
            string? keyword,
            int page,
            int pageSize);

        Task<Tenant?> GetByIdAsync(int id, bool includeRelations = false);

        Task<Tenant?> GetByNumAsync(string tenantNum);

        Task<bool> ExistsAsync(int id);

        Task<bool> NumExistsAsync(string tenantNum, int? excludeId = null);

        Task<int> GetAccountCountAsync(int tenantId);

        Task<Tenant> CreateAsync(Tenant tenant);

        Task<bool> UpdateAsync(Tenant tenant);

        Task<bool> ToggleStatusAsync(int id, bool isEnable);

        Task<List<Tenant>> GetAllActiveAsync();
    }
}