using ADAMS.Areas.Models;
using ADAMS.Areas.OperationManagement.ViewModels.FeedingRecordQuery;

namespace ADAMS.Areas.OperationManagement.Services.FeedingRecordQuery
{
    public interface IFeedingRecordService
    {
        Task<FeedingRecordListViewModel> GetListViewModelAsync(
            int? tenantSN,
            int? areaSN,
            DateTime? startDate,
            DateTime? endDate);

        Task<FeedingRecordEditViewModel> GetCreateViewModelAsync(
            int? tenantSN,
            int? areaSN,
            int? pondSN);

        Task<FeedingRecordEditViewModel?> GetEditViewModelAsync(int id);

        Task CreateAsync(FeedingRecordEditViewModel model);
        Task UpdateAsync(FeedingRecordEditViewModel model);
        Task SoftDeleteAsync(int id);

        Task<byte[]> ExportCsvAsync(int? tenantSN, int? areaSN, DateTime? startDate, DateTime? endDate);

        Task<List<PondOption>> GetPondOptionsAsync(int tenantSN, int areaSN);
        Task<List<FeedOption>> GetFeedOptionsAsync(int tenantSN, int supplierSN);
    }
}
