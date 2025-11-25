using ADAMS.Areas.BasicDataManagement.ViewModels.TimeSegmentSetting;

namespace ADAMS.Areas.BasicDataManagement.Services.TimeSegmentSetting
{
    public interface ITimeSegmentService
    {
        Task<TimeZoneListViewModel> GetListViewModelAsync(
            int? tenantSN,
            string? keyword,
            int page,
            int pageSize);

        Task<TimeZoneEditViewModel> GetCreateViewModelAsync(int? tenantSNFromQuery = null);

        Task<TimeZoneEditViewModel?> GetEditViewModelAsync(int timeZoneSN);

        Task<(bool Success, string Message)> CreateAsync(TimeZoneEditViewModel model, string currentUser);

        Task<(bool Success, string Message)> UpdateAsync(TimeZoneEditViewModel model, string currentUser);

        Task SoftDeleteAsync(int timeZoneSN, string currentUser);
    }
}
