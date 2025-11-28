using System.Collections.Generic;
using System.Threading.Tasks;
using ADAMS.Models;

namespace ADAMS.Areas.BasicDataManagement.Repositories.UnitDataManagement
{
    public interface IUnitDataRepository
    {
        Task<List<Unit>> GetListAsync(string? keyword);
        Task<Unit?> GetByIdAsync(int unitSn);
        Task<bool> ExistsByNameAsync(string unitName, int? excludeUnitSn = null);
        Task AddAsync(Unit entity);
        Task UpdateAsync(Unit entity);
        Task SoftDeleteAsync(int unitSn, string currentUser);
    }
}
