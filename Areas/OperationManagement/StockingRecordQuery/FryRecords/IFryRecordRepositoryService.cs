using ADAMS.Areas.OperationManagement.ViewModels.FryRecords;

namespace ADAMS.Areas.OperationManagement.Services.FryRecords
{
    public interface IFryRecordService
    {
        Task<FryRecordListViewModel> GetPagedListAsync(
            int tenantSN,
            int? areaSN = null,
            int? pondSN = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int page = 1,
            int pageSize = 20);

        Task<FryRecordDetailViewModel?> GetDetailAsync(int id);

        Task<(bool Success, string Message, int? FryRecordSN)> CreateAsync(
            FryRecordCreateViewModel model,
            string currentUser);

        Task<(bool Success, string Message)> UpdateAsync(
            FryRecordEditViewModel model,
            string currentUser);

        Task<(bool Success, string Message)> SoftDeleteAsync(
            int id,
            string currentUser);

        Task<List<FryRecordDropdownDto>> GetDropdownListAsync(int tenantSN);
    }
}
