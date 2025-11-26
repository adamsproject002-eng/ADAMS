using ADAMS.Areas.BasicDataManagement.Repositories.AreaPondManagement;
using ADAMS.Areas.BasicDataManagement.ViewModels.AreaPondManagement;
using ADAMS.Areas.Models;
using ADAMS.Models;
using ADAMS.Services;

namespace ADAMS.Areas.BasicDataManagement.Services.AreaPondManagement
{
    public class AreaPondService : IAreaPondService
    {
        private readonly IAreaPondRepository _repo;
        private readonly ICurrentAccountService _current;

        public AreaPondService(
            IAreaPondRepository repo,
            ICurrentAccountService current)
        {
            _repo = repo;
            _current = current;
        }

        private (bool isOp, int tenantSN) GetTenantContext(int? tenantSNFromQuery)
        {
            if (_current.IsOperationCompany)
            {
                var sn = tenantSNFromQuery ?? _current.TenantSN;
                return (true, sn);
            }
            else
            {
                return (false, _current.TenantSN);
            }
        }

        // ---------- Area List ----------

        public async Task<AreaListViewModel> GetAreaListViewModelAsync(
            int? tenantSN,
            string? keyword,
            int page,
            int pageSize)
        {
            var (isOp, currentTenantSN) = GetTenantContext(tenantSN);

            var tenantOptions = await _repo.GetTenantOptionsAsync(isOp, currentTenantSN);

            if ((currentTenantSN == 0 || !tenantOptions.Any(t => t.SN == currentTenantSN))
                && tenantOptions.Any())
            {
                currentTenantSN = tenantOptions.First().SN;
            }

            var (data, totalCount) = await _repo.GetAreaPagedListAsync(
                currentTenantSN, keyword, page, pageSize);

            var items = data.Select(a => new AreaListItemViewModel
            {
                AreaSN = a.AreaSN,
                AreaName = a.AreaName,
                GPSX = a.GPSX,
                GPSY = a.GPSY,
                PondCount = a.Ponds.Count(p => !p.IsDeleted),
                TotalPondArea = a.Ponds.Where(p => !p.IsDeleted).Sum(p => p.PondArea),
                Remark = a.Remark
            }).ToList();

            return new AreaListViewModel
            {
                IsOperationCompany = isOp,
                CurrentTenantSN = currentTenantSN,
                TenantOptions = tenantOptions,
                Keyword = keyword,
                Areas = items,
                Pagination = new PaginationInfo
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    SearchFilters = new Dictionary<string, object>
                    {
                        ["tenantSN"] = currentTenantSN,
                        ["keyword"] = keyword ?? string.Empty
                    }
                }
            };
        }

        // ---------- Area Edit ----------

        public async Task<AreaEditViewModel> GetAreaEditViewModelAsync(int? areaSN, int? tenantSN)
        {
            var (isOp, currentTenantSN) = GetTenantContext(tenantSN);
            var tenantOptions = await _repo.GetTenantOptionsAsync(isOp, currentTenantSN);

            Area? area = null;
            if (areaSN.HasValue)
            {
                area = await _repo.GetAreaAsync(areaSN.Value);
                if (area == null)
                    throw new InvalidOperationException("找不到指定的養殖區域。");
            }

            var model = new AreaEditViewModel
            {
                IsOperationCompany = isOp,
                TenantOptions = tenantOptions
            };

            if (area == null)
            {
                // 新增
                model.TenantSN = currentTenantSN;
                model.TenantName = tenantOptions.FirstOrDefault(t => t.SN == currentTenantSN)?.TenantName ?? "";
            }
            else
            {
                model.AreaSN = area.AreaSN;
                model.AreaName = area.AreaName;
                model.GPSX = area.GPSX;
                model.GPSY = area.GPSY;
                model.Remark = area.Remark;
                model.TenantSN = area.TenantSN;
                model.TenantName = tenantOptions.FirstOrDefault(t => t.SN == area.TenantSN)?.TenantName ?? "";
            }

            return model;
        }

        public async Task CreateAreaAsync(AreaEditViewModel model, string currentUser)
        {
            var entity = new Area
            {
                TenantSN = model.TenantSN,
                AreaName = model.AreaName,
                GPSX = model.GPSX,
                GPSY = model.GPSY,
                Remark = model.Remark,
                IsDeleted = false,
                CreateTime = DateTime.Now,
                CreateUser = currentUser
            };

            await _repo.AddAreaAsync(entity);
        }

        public async Task UpdateAreaAsync(AreaEditViewModel model, string currentUser)
        {
            var entity = await _repo.GetAreaAsync(model.AreaSN!.Value)
                ?? throw new InvalidOperationException("找不到指定的養殖區域。");

            entity.AreaName = model.AreaName;
            entity.GPSX = model.GPSX;
            entity.GPSY = model.GPSY;
            entity.Remark = model.Remark;
            entity.ModifyTime = DateTime.Now;
            entity.ModifyUser = currentUser;

            await _repo.UpdateAreaAsync(entity);
        }

        public async Task DeleteAreaAsync(int areaSN, string currentUser)
        {
            if (await _repo.HasPondsAsync(areaSN))
            {
                throw new InvalidOperationException("此養殖區仍有養殖池資料，無法刪除。");
            }

            await _repo.SoftDeleteAreaAsync(areaSN, currentUser);
        }

        // ---------- Pond List ----------

        public async Task<PondListViewModel> GetPondListViewModelAsync(
            int areaSN,
            string? keyword,
            int page,
            int pageSize)
        {
            var area = await _repo.GetAreaAsync(areaSN)
                ?? throw new InvalidOperationException("找不到指定的養殖區域。");

            var (data, totalCount, totalArea) =
                await _repo.GetPondsPagedListAsync(areaSN, keyword, page, pageSize);

            var items = data.Select(p => new PondListItemViewModel
            {
                PondSN = p.PondSN,
                PondNum = p.PondNum,
                PondWidth = p.PondWidth,
                PondLength = p.PondLength,
                PondArea = p.PondArea,
                Remark = p.Remark
            }).ToList();

            return new PondListViewModel
            {
                AreaSN = area.AreaSN,
                AreaName = area.AreaName,
                TotalPondArea = totalArea,
                Keyword = keyword,
                Ponds = items,
                Pagination = new PaginationInfo
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    SearchFilters = new Dictionary<string, object>
                    {
                        ["areaSN"] = areaSN,
                        ["keyword"] = keyword ?? string.Empty
                    }
                }
            };
        }

        // ---------- Pond Edit ----------

        public async Task<PondEditViewModel> GetPondEditViewModelAsync(int? pondSN, int areaSN)
        {
            var area = await _repo.GetAreaAsync(areaSN)
                ?? throw new InvalidOperationException("找不到指定的養殖區域。");

            Pond? pond = null;
            if (pondSN.HasValue)
            {
                pond = await _repo.GetPondAsync(pondSN.Value);
                if (pond == null)
                    throw new InvalidOperationException("找不到指定的養殖池。");
            }

            var vm = new PondEditViewModel
            {
                AreaSN = area.AreaSN,
                AreaName = area.AreaName,
                TenantSN = area.TenantSN
            };

            if (pond != null)
            {
                vm.PondSN = pond.PondSN;
                vm.PondNum = pond.PondNum;
                vm.PondWidth = pond.PondWidth;
                vm.PondLength = pond.PondLength;
                vm.PondArea = pond.PondArea;
                vm.GPSX = pond.GPSX;
                vm.GPSY = pond.GPSY;
                vm.Remark = pond.Remark;
            }

            return vm;
        }

        public async Task CreatePondAsync(PondEditViewModel model, string currentUser)
        {
            var entity = new Pond
            {
                AreaSN = model.AreaSN,
                TenantSN = model.TenantSN,
                PondNum = model.PondNum,
                PondWidth = model.PondWidth,
                PondLength = model.PondLength,
                PondArea = model.PondArea,
                GPSX = model.GPSX,
                GPSY = model.GPSY,
                Remark = model.Remark,
                IsDeleted = false,
                CreateTime = DateTime.Now,
                CreateUser = currentUser
            };

            await _repo.AddPondAsync(entity);
        }

        public async Task UpdatePondAsync(PondEditViewModel model, string currentUser)
        {
            var entity = await _repo.GetPondAsync(model.PondSN!.Value)
                ?? throw new InvalidOperationException("找不到指定的養殖池。");

            entity.PondNum = model.PondNum;
            entity.PondWidth = model.PondWidth;
            entity.PondLength = model.PondLength;
            entity.PondArea = model.PondArea;
            entity.GPSX = model.GPSX;
            entity.GPSY = model.GPSY;
            entity.Remark = model.Remark;
            entity.ModifyTime = DateTime.Now;
            entity.ModifyUser = currentUser;

            await _repo.UpdatePondAsync(entity);
        }

        public async Task DeletePondAsync(int pondSN, string currentUser)
        {
            await _repo.SoftDeletePondAsync(pondSN, currentUser);
        }
    }
}
