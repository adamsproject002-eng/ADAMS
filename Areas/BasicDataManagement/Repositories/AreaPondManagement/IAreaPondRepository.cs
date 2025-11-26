using ADAMS.Areas.Models;
using ADAMS.Models;

namespace ADAMS.Areas.BasicDataManagement.Repositories.AreaPondManagement
{
    public interface IAreaPondRepository
    {
        // 場區 Area 相關
        Task<(List<Area> Data, int TotalCount)> GetAreaPagedListAsync(
            int tenantSN,
            string? keyword,
            int page,
            int pageSize);

        Task<List<TenantOption>> GetTenantOptionsAsync(
      bool isOperationCompany,
      int currentTenantSN);

        Task<Area?> GetAreaAsync(int areaSN);
        Task AddAreaAsync(Area area);
        Task UpdateAreaAsync(Area area);
        Task<bool> HasPondsAsync(int areaSN);
        Task SoftDeleteAreaAsync(int areaSN, string currentUser);

        // 養殖池 Pond 相關
        Task<(List<Pond> Data, int TotalCount, decimal TotalArea)> GetPondsPagedListAsync(
            int areaSN,
            string? keyword,
            int page,
            int pageSize);

        Task<Pond?> GetPondAsync(int pondSN);
        Task AddPondAsync(Pond pond);
        Task UpdatePondAsync(Pond pond);
        Task SoftDeletePondAsync(int pondSN, string currentUser);
    }
}
