using ADAMS.Areas.BasicDataManagement.ViewModels.SeedlingDataManagement;

namespace ADAMS.Areas.BasicDataManagement.Services.SeedlingDataManagement
{
    public interface ISeedlingDataService
    {
        Task<SeedlingListViewModel> GetListViewModelAsync(
            int? tenantSN,
            int selectedFVSN,
            string? keyword,
            int page,
            int pageSize);

        Task<SeedlingEditViewModel> GetCreateViewModelAsync();

        Task<SeedlingEditViewModel?> GetEditViewModelAsync(int frySN);

        Task CreateAsync(SeedlingEditViewModel model);

        Task UpdateAsync(SeedlingEditViewModel model);

        Task DeleteAsync(int frySN);
    }
}
