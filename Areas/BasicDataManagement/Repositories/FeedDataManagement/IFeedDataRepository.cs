using ADAMS.Areas.Models;
using ADAMS.Models;

namespace ADAMS.Areas.BasicDataManagement.Repositories.FeedDataManagement
{
    public interface IFeedDataRepository
    {
        Task<List<TenantOption>> GetTenantOptionsAsync(bool isOperationCompany, int currentTenantSN);

        Task<List<Supplier>> GetSuppliersForTenantAsync(int tenantSN);

        Task<(List<Feed> Data, int TotalCount)> GetPagedListAsync(
            int tenantSN,
            string? keyword,
            int page,
            int pageSize);

        Task<Feed?> GetAsync(int feedSN);

        Task CreateAsync(Feed feed);

        Task UpdateAsync(Feed feed);

        Task SoftDeleteAsync(int feedSN, string currentUser);
    }
}
