using ADAMS.Models;

namespace ADAMS.Areas.BasicDataManagement.Repositories.FarmingTypeManagement
{
    public interface IFarmingTypeRepository
    {
        Task<List<FishVariety>> GetListAsync(string? keyword);
        Task<FishVariety?> GetByIdAsync(int fvsn);
        Task<bool> ExistsByNameAsync(string fvName, int? excludeFvsn = null);
        Task AddAsync(FishVariety entity);
        Task UpdateAsync(FishVariety entity);
        Task SoftDeleteAsync(int fvsn, string currentUser);
    }
}
