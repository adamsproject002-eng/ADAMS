using ADAMS.Areas.BasicDataManagement.ViewModels.FeedingPlanManagement;

namespace ADAMS.Areas.BasicDataManagement.Services.FeedingPlanManagement
{
    public interface IFeedingPlanService
    {
        Task<FeedingPlanListViewModel> GetListViewModelAsync(int? tenantSN, int? fvsn);
        Task<FeedingPlanEditViewModel> GetCreateViewModelAsync(int? tenantSN, int? fvsn);
        Task<FeedingPlanEditViewModel?> GetEditViewModelAsync(int fpdsn);

        Task CreateAsync(FeedingPlanEditViewModel model);
        Task UpdateAsync(FeedingPlanEditViewModel model);
        Task SoftDeleteAsync(int fpdsn);
    }
}
