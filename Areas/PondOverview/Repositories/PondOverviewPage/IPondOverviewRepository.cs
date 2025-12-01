using ADAMS.Areas.Models;
using ADAMS.Areas.PondOverview.ViewModels.PondOverviewPage;

namespace ADAMS.Areas.PondOverview.Repositories.PondOverviewPage
{
    public interface IPondOverviewRepository
    {
        /// <summary>
        /// 給所有「養殖戶下拉選單」共用的方法
        /// </summary>
        Task<List<TenantOption>> GetTenantOptionsAsync(bool isOperationCompany, int currentTenantSN);

        /// <summary>
        /// 取得某養殖戶底下的場區清單
        /// </summary>
        Task<List<AreaOption>> GetAreaOptionsAsync(int tenantSN);

        /// <summary>
        /// 取得某養殖戶 + 場區的養殖池卡片資料
        /// </summary>
        Task<List<PondCardViewModel>> GetPondCardsAsync(int tenantSN, int? areaSN);

        /// <summary>
        /// 取得單一養殖池的詳細資料
        /// </summary>
        Task<PondDetailViewModel?> GetPondDetailAsync(int pondSN);
    }
}
