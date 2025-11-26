using ADAMS.Areas.BasicDataManagement.ViewModels.AreaPondManagement;

namespace ADAMS.Areas.BasicDataManagement.Services.AreaPondManagement
{
    public interface IAreaPondService
    {
        Task<AreaListViewModel> GetAreaListViewModelAsync(
            int? tenantSN,
            string? keyword,
            int page,
            int pageSize);

        Task<AreaEditViewModel> GetAreaEditViewModelAsync(int? areaSN, int? tenantSN);
        Task CreateAreaAsync(AreaEditViewModel model, string currentUser);
        Task UpdateAreaAsync(AreaEditViewModel model, string currentUser);
        Task DeleteAreaAsync(int areaSN, string currentUser);

        Task<PondListViewModel> GetPondListViewModelAsync(
            int areaSN,
            string? keyword,
            int page,
            int pageSize);

        Task<PondEditViewModel> GetPondEditViewModelAsync(int? pondSN, int areaSN);
        Task CreatePondAsync(PondEditViewModel model, string currentUser);
        Task UpdatePondAsync(PondEditViewModel model, string currentUser);
        Task DeletePondAsync(int pondSN, string currentUser);
    }
}
