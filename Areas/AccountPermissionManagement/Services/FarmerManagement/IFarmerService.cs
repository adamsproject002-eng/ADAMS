using ADAMS.Areas.AccountPermissionManagement.ViewModels.FarmerManagement;

namespace ADAMS.Areas.AccountPermissionManagement.Services.FarmerManagement
{
    public interface IFarmerService
    {
        Task<FarmerListViewModel> GetPagedListAsync(string? statusFilter, string? Keyword, int page, int pageSize);

        Task<FarmerDetailViewModel?> GetDetailAsync(int id);

        Task<(bool Success, string Message, int? FarmerId)> CreateAsync(
            FarmerCreateViewModel model,
            string currentUser);

        Task<(bool Success, string Message)> UpdateAsync(
            FarmerEditViewModel model,
            string currentUser);

        Task<(bool Success, string Message)> ToggleStatusAsync(int id, bool isEnable);

        Task<List<FarmerDropdownDto>> GetDropdownListAsync();
    }
}
