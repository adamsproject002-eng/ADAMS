using ADAMS.Areas.Models;

namespace ADAMS.Areas.BasicDataManagement.Repositories.TimeSegmentSetting
{
    public interface ITimeSegmentRepository
    {
        Task<(List<ADAMS.Models.TimeZone> Data, int TotalCount)> GetPagedListAsync(
            int tenantSN,
            string? keyword,
            int page,
            int pageSize);

        Task<List<TenantOption>> GetTenantOptionsAsync(
            bool isOperationCompany,
            int currentTenantSN);

        Task<ADAMS.Models.TimeZone?> GetByIdAsync(int timeZoneSN);

        Task<bool> CheckTimeZoneNumExistsAsync(string timeZoneNum, int tenantSN, int? excludeTimeZoneSN = null);

        Task AddAsync(ADAMS.Models.TimeZone timeZone);
        Task UpdateAsync(ADAMS.Models.TimeZone timeZone);
        Task SoftDeleteAsync(int timeZoneSN, string currentUser);
    }
}
