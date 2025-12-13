using ADAMS.Areas.Models;
using ADAMS.Areas.OperationManagement.ViewModels.FeedingRecordQuery;
using ADAMS.Areas.PondOverview.ViewModels.PondOverviewPage;
using ADAMS.Models;
using System.Data;

namespace ADAMS.Areas.OperationManagement.Repositories.FeedingRecordQuery
{
    public interface IFeedingRecordRepository
    {
        Task<List<TenantOption>> GetTenantOptionsAsync(bool isOperationCompany,int currentTenantSN);
        Task<List<FeedingRecordListItemViewModel>> SearchAsync(
            int tenantSN,
            int? areaSN,
            DateTime? startDate,
            DateTime? endDate);

        Task<FeedingRecord?> GetAsync(int id);

        Task AddAsync(FeedingRecord entity);
        Task UpdateAsync(FeedingRecord entity);
        Task SoftDeleteAsync(int id, string userName);

        // 下拉資料
        Task<List<AreaOption>> GetAreaOptionsAsync(int tenantSN);
        Task<List<PondOption>> GetPondOptionsAsync(int tenantSN, int? areaSN);
        Task<List<TimeZoneOptionVM>> GetTimeZoneOptionsAsync(int tenantSN);
        Task<List<FeedOption>> GetFeedOptionsAsync(int tenantSN);
        Task<List<ManageAccountOptionVM>> GetManageAccountOptionsAsync(int tenantSN);

        // 取得該池最近放養資訊（放養碼、魚種、ABW/DOC等），目前先回傳基本資料即可
        Task<(string FarmingCode, string FishVarietyName, decimal ABW, int DOC, int StockingQty)?>
            GetLatestFarmingInfoAsync(int pondSN, DateTime feedingDate);

        Task<List<SupplierOption>> GetFeedSuppliersAsync(int tenantSN);
        Task<List<FeedOption>> GetFeedsBySupplierAsync(int tenantSN, int supplierSN);
    }
}
