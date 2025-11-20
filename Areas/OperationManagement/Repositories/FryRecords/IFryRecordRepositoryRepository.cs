using ADAMS.Models;

namespace ADAMS.Areas.OperationManagement.Repositories.FryRecords
{
    public interface IFryRecordRepository
    {
        Task<(List<FryRecord> Data, int TotalCount)> GetPagedListAsync(
            int tenantSN,
            int? areaSN = null,
            int? pondSN = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int page = 1,
            int pageSize = 20);

        Task<FryRecord?> GetByIdAsync(int id);

        Task<FryRecord> CreateAsync(FryRecord fryRecord);

        Task<bool> UpdateAsync(FryRecord fryRecord);

        Task<bool> SoftDeleteAsync(int id, string deleteUser);

        Task<List<FryRecord>> GetAllActiveAsync(int tenantSN);
    }
}
