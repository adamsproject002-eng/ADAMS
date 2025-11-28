using ADAMS.Areas.BasicDataManagement.ViewModels.FarmingTypeManagement;

namespace ADAMS.Areas.BasicDataManagement.Services.FarmingTypeManagement
{
    public interface IFarmingTypeService
    {
        Task<FarmingTypeListViewModel> GetListViewModelAsync(string? keyword);
        Task<FarmingTypeEditViewModel> GetCreateViewModelAsync();
        Task<FarmingTypeEditViewModel?> GetEditViewModelAsync(int fvsn);
        Task CreateAsync(FarmingTypeEditViewModel model);
        Task UpdateAsync(FarmingTypeEditViewModel model);
        Task SoftDeleteAsync(int fvsn);
    }
}
