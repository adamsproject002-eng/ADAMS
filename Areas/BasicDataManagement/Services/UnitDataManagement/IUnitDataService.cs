using System.Threading.Tasks;
using ADAMS.Areas.BasicDataManagement.ViewModels.UnitDataManagement;

namespace ADAMS.Areas.BasicDataManagement.Services.UnitDataManagement
{
    public interface IUnitDataService
    {
        Task<UnitDataListViewModel> GetListViewModelAsync(string? keyword);
        Task<UnitDataEditViewModel> GetCreateViewModelAsync();
        Task<UnitDataEditViewModel?> GetEditViewModelAsync(int unitSn);
        Task CreateAsync(UnitDataEditViewModel model);
        Task UpdateAsync(UnitDataEditViewModel model);
        Task SoftDeleteAsync(int unitSn);
    }
}
