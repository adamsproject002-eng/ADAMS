using ADAMS.Models;

namespace ADAMS.Areas.BasicDataManagement.Repositories.GrowthTargetReference
{
    public interface IGrowthTargetRepository
    {
        Task<List<FishVariety>> GetFishVarietiesAsync();
        Task<GrowTargetMain?> GetMainByFVSNAsync(int fvsn);
        Task<GrowTargetMain> CreateMainAsync(int fvsn, string gtmName, string currentUser);

        Task<List<GrowTargetDetail>> GetDetailsAsync(int gtmsn);
        Task<GrowTargetDetail?> GetDetailAsync(int gtdsn);

        Task AddDetailAsync(GrowTargetDetail detail);
        Task UpdateDetailAsync(GrowTargetDetail detail);
        Task SoftDeleteDetailAsync(int gtdsn, string currentUser);
    }
}
