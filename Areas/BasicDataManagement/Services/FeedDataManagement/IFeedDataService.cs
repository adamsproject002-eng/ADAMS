using ADAMS.Areas.BasicDataManagement.ViewModels.FeedDataManagement;

namespace ADAMS.Areas.BasicDataManagement.Services.FeedDataManagement
{
    public interface IFeedDataService
    {
        Task<FeedListViewModel> GetListViewModelAsync(
            int? tenantSN,
            string? keyword,
            int page,
            int pageSize);

        Task<FeedEditViewModel> GetCreateViewModelAsync();

        Task<FeedEditViewModel?> GetEditViewModelAsync(int feedSN);

        Task<bool> CreateAsync(FeedEditViewModel model);

        Task<bool> UpdateAsync(FeedEditViewModel model);

        Task<bool> SoftDeleteAsync(int feedSN);
    }
}
