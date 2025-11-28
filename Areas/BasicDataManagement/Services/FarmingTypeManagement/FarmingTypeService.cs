using ADAMS.Areas.BasicDataManagement.Repositories.FarmingTypeManagement;
using ADAMS.Areas.BasicDataManagement.ViewModels.FarmingTypeManagement;
using ADAMS.Models;
using ADAMS.Services;

namespace ADAMS.Areas.BasicDataManagement.Services.FarmingTypeManagement
{
    public class FarmingTypeService : IFarmingTypeService
    {
        private readonly IFarmingTypeRepository _repo;
        private readonly ICurrentAccountService _current;

        public FarmingTypeService(
            IFarmingTypeRepository repo,
            ICurrentAccountService current)
        {
            _repo = repo;
            _current = current;
        }

        public async Task<FarmingTypeListViewModel> GetListViewModelAsync(string? keyword)
        {
            var items = await _repo.GetListAsync(keyword);

            return new FarmingTypeListViewModel
            {
                IsOperationCompany = _current.IsOperationCompany,
                Keyword = keyword,
                Items = items.Select(v => new FarmingTypeListItemViewModel
                {
                    FVSN = v.FVSN,
                    FVName = v.FVName,
                    Remark = v.Remark
                }).ToList()
            };
        }

        public Task<FarmingTypeEditViewModel> GetCreateViewModelAsync()
        {
            var vm = new FarmingTypeEditViewModel
            {
                IsOperationCompany = _current.IsOperationCompany
            };
            return Task.FromResult(vm);
        }

        public async Task<FarmingTypeEditViewModel?> GetEditViewModelAsync(int fvsn)
        {
            var entity = await _repo.GetByIdAsync(fvsn);
            if (entity == null) return null;

            return new FarmingTypeEditViewModel
            {
                FVSN = entity.FVSN,
                FVName = entity.FVName,
                Remark = entity.Remark,
                IsOperationCompany = _current.IsOperationCompany
            };
        }

        public async Task CreateAsync(FarmingTypeEditViewModel model)
        {
            EnsureOperationCompany();

            // 名稱不得重複
            if (await _repo.ExistsByNameAsync(model.FVName))
            {
                throw new InvalidOperationException("此魚種名稱已存在。");
            }

            var entity = new FishVariety
            {
                FVName = model.FVName.Trim(),
                Remark = string.IsNullOrWhiteSpace(model.Remark) ? null : model.Remark.Trim(),
                IsDeleted = false,
                CreateTime = DateTime.Now,
                CreateUser = _current.Name
            };

            await _repo.AddAsync(entity);
        }

        public async Task UpdateAsync(FarmingTypeEditViewModel model)
        {
            EnsureOperationCompany();

            if (!model.FVSN.HasValue)
                throw new InvalidOperationException("缺少魚種序號。");

            // 名稱不得與其他筆重複
            if (await _repo.ExistsByNameAsync(model.FVName, model.FVSN))
            {
                throw new InvalidOperationException("此魚種名稱已存在。");
            }

            var entity = await _repo.GetByIdAsync(model.FVSN.Value);
            if (entity == null) return;

            entity.FVName = model.FVName.Trim();
            entity.Remark = string.IsNullOrWhiteSpace(model.Remark) ? null : model.Remark.Trim();
            entity.ModifyTime = DateTime.Now;
            entity.ModifyUser = _current.Name;

            await _repo.UpdateAsync(entity);
        }

        public async Task SoftDeleteAsync(int fvsn)
        {
            EnsureOperationCompany();
            await _repo.SoftDeleteAsync(fvsn, _current.Name);
        }

        private void EnsureOperationCompany()
        {
            if (!_current.IsOperationCompany)
            {
                throw new InvalidOperationException("只有營運公司帳號可以維護魚種資料。");
            }
        }
    }
}
