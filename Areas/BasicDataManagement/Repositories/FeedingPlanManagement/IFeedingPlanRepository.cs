using ADAMS.Areas.Models;
using ADAMS.Models;


namespace ADAMS.Areas.BasicDataManagement.Repositories.FeedingPlanManagement
{
    public interface IFeedingPlanRepository
    {
        Task<List<TenantOption>> GetTenantOptionsAsync(bool isOperationCompany, int currentTenantSN);
        Task<List<FishVariety>> GetFishVarietiesAsync();

        Task<FeedingPlanMain?> GetMainAsync(int tenantSN, int fvsn);
        Task<FeedingPlanMain> CreateMainAsync(int tenantSN, int fvsn, string planName, string currentUser);

        Task<List<FeedingPlanDetail>> GetDetailsAsync(int fpmsn);
        Task<FeedingPlanDetail?> GetDetailAsync(int fpdsn);

        Task AddDetailAsync(FeedingPlanDetail detail);
        Task UpdateDetailAsync(FeedingPlanDetail detail);
        Task SoftDeleteDetailAsync(int fpdsn, string currentUser);
    }
}
