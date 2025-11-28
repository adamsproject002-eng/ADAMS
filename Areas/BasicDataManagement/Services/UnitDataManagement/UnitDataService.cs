using System;
using System.Linq;
using System.Threading.Tasks;
using ADAMS.Areas.BasicDataManagement.Repositories.UnitDataManagement;
using ADAMS.Areas.BasicDataManagement.ViewModels.UnitDataManagement;
using ADAMS.Models;
using ADAMS.Services;

namespace ADAMS.Areas.BasicDataManagement.Services.UnitDataManagement
{
    public class UnitDataService : IUnitDataService
    {
        private readonly IUnitDataRepository _repo;
        private readonly ICurrentAccountService _current;

        public UnitDataService(
            IUnitDataRepository repo,
            ICurrentAccountService current)
        {
            _repo = repo;
            _current = current;
        }

        public async Task<UnitDataListViewModel> GetListViewModelAsync(string? keyword)
        {
            var items = await _repo.GetListAsync(keyword);

            return new UnitDataListViewModel
            {
                IsOperationCompany = _current.IsOperationCompany,
                Keyword = keyword,
                Items = items.Select(u => new UnitDataListItemViewModel
                {
                    UnitSN = u.UnitSN,
                    UnitName = u.UnitName,
                    Remark = u.Remark
                }).ToList()
            };
        }

        public Task<UnitDataEditViewModel> GetCreateViewModelAsync()
        {
            var vm = new UnitDataEditViewModel
            {
                IsOperationCompany = _current.IsOperationCompany
            };
            return Task.FromResult(vm);
        }

        public async Task<UnitDataEditViewModel?> GetEditViewModelAsync(int unitSn)
        {
            var entity = await _repo.GetByIdAsync(unitSn);
            if (entity == null) return null;

            return new UnitDataEditViewModel
            {
                UnitSN = entity.UnitSN,
                UnitName = entity.UnitName,
                Remark = entity.Remark,
                IsOperationCompany = _current.IsOperationCompany
            };
        }

        public async Task CreateAsync(UnitDataEditViewModel model)
        {
            EnsureOperationCompany();

            if (await _repo.ExistsByNameAsync(model.UnitName))
            {
                throw new InvalidOperationException("此單位名稱已存在。");
            }

            var entity = new Unit
            {
                UnitName = model.UnitName.Trim(),
                Remark = string.IsNullOrWhiteSpace(model.Remark) ? null : model.Remark.Trim(),
                IsDeleted = false,
                CreateTime = DateTime.Now,
                CreateUser = _current.Name
            };

            await _repo.AddAsync(entity);
        }

        public async Task UpdateAsync(UnitDataEditViewModel model)
        {
            EnsureOperationCompany();

            if (!model.UnitSN.HasValue)
                throw new InvalidOperationException("缺少單位序號。");

            if (await _repo.ExistsByNameAsync(model.UnitName, model.UnitSN))
            {
                throw new InvalidOperationException("此單位名稱已存在。");
            }

            var entity = await _repo.GetByIdAsync(model.UnitSN.Value);
            if (entity == null) return;

            entity.UnitName = model.UnitName.Trim();
            entity.Remark = string.IsNullOrWhiteSpace(model.Remark) ? null : model.Remark.Trim();
            entity.ModifyTime = DateTime.Now;
            entity.ModifyUser = _current.Name;

            await _repo.UpdateAsync(entity);
        }

        public async Task SoftDeleteAsync(int unitSn)
        {
            EnsureOperationCompany();
            await _repo.SoftDeleteAsync(unitSn, _current.Name);
        }

        private void EnsureOperationCompany()
        {
            if (!_current.IsOperationCompany)
            {
                throw new InvalidOperationException("只有營運公司帳號可以維護單位資料。");
            }
        }
    }
}
