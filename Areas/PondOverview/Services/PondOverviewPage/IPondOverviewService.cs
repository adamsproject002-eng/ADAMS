using ADAMS.Areas.PondOverview.ViewModels.PondOverviewPage;

namespace ADAMS.Areas.PondOverview.Services.PondOverviewPage
{
    public interface IPondOverviewService
    {
        Task<PondOverviewListViewModel> GetOverviewAsync(int? tenantSN, int? areaSN);
        Task<PondDetailViewModel?> GetDetailViewModelAsync(int pondSN);
    }
}
