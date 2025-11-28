using ADAMS.Areas.BasicDataManagement.ViewModels.GrowthTargetReference;

namespace ADAMS.Areas.BasicDataManagement.Services.GrowthTargetReference
{
    public interface IGrowthTargetService
    {
        Task<GrowthTargetListViewModel> GetListViewModelAsync(int? fvsn);
        Task<GrowthTargetEditViewModel> GetCreateViewModelAsync(int fvsn);
        Task<GrowthTargetEditViewModel?> GetEditViewModelAsync(int gtdsn);

        Task CreateAsync(GrowthTargetEditViewModel model);
        Task UpdateAsync(GrowthTargetEditViewModel model);
        Task SoftDeleteAsync(int gtdsn);
    }
}
