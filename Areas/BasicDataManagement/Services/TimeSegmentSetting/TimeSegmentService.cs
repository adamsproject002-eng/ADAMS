using ADAMS.Areas.BasicDataManagement.Repositories.TimeSegmentSetting;
using ADAMS.Areas.BasicDataManagement.ViewModels.TimeSegmentSetting;
using ADAMS.Areas.Models;
using ADAMS.Services;

namespace ADAMS.Areas.BasicDataManagement.Services.TimeSegmentSetting
{
    public class TimeSegmentService : ITimeSegmentService
    {
        private readonly ITimeSegmentRepository _repo;
        private readonly ICurrentAccountService _current;

        public TimeSegmentService(
            ITimeSegmentRepository repo,
            ICurrentAccountService current)
        {
            _repo = repo;
            _current = current;
        }

        private (bool isOperationCompany, int tenantSN) GetTenantContext(int? fromQuery)
        {
            if (_current.IsOperationCompany)
            {
                var tenantSN = fromQuery ?? _current.TenantSN;
                return (true, tenantSN);
            }

            return (false, _current.TenantSN);
        }

        public async Task<TimeZoneListViewModel> GetListViewModelAsync(
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

            var (data, totalCount) = await _repo.GetPagedListAsync(
                currentTenantSN, keyword, page, pageSize);

            var items = data.Select(t => new TimeZoneListItemViewModel
            {
                TimeZoneSN = t.TimeZoneSN,
                TimeZoneNum = t.TimeZoneNum,
                TimeZoneDesc = t.TimeZoneDesc,
                Remark = t.Remark
            }).ToList();

            var vm = new TimeZoneListViewModel
            {
                IsOperationCompany = isOp,
                CurrentTenantSN = currentTenantSN,
                TenantOptions = tenantOptions,
                Keyword = keyword,
                TimeZones = items,
                Pagination = new PaginationInfo
                {
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = totalCount
                }
            };

            return vm;
        }

        public async Task<TimeZoneEditViewModel> GetCreateViewModelAsync(int? tenantSNFromQuery = null)
        {
            var (isOp, currentTenantSN) = GetTenantContext(tenantSNFromQuery);

            var tenantOptions = await _repo.GetTenantOptionsAsync(isOp, currentTenantSN);

            if ((currentTenantSN == 0 || !tenantOptions.Any(t => t.SN == currentTenantSN))
                && tenantOptions.Any())
            {
                currentTenantSN = tenantOptions.First().SN;
            }

            var currentTenant = tenantOptions.FirstOrDefault(t => t.SN == currentTenantSN);
            var tenantName = currentTenant?.TenantName ?? string.Empty;

            return new TimeZoneEditViewModel
            {
                TimeZoneSN = null,
                TenantSN = currentTenantSN,
                TenantName = tenantName,
                IsOperationCompany = isOp,
                TenantOptions = tenantOptions
            };
        }

        public async Task<TimeZoneEditViewModel?> GetEditViewModelAsync(int timeZoneSN)
        {
            var timeZone = await _repo.GetByIdAsync(timeZoneSN);
            if (timeZone == null)
                return null;

            var (isOp, _) = GetTenantContext(timeZone.TenantSN);
            var tenantOptions = await _repo.GetTenantOptionsAsync(isOp, timeZone.TenantSN);
            var currentTenant = tenantOptions.FirstOrDefault(t => t.SN == timeZone.TenantSN);
            var tenantName = currentTenant?.TenantName ?? string.Empty;

            return new TimeZoneEditViewModel
            {
                TimeZoneSN = timeZone.TimeZoneSN,
                TenantSN = timeZone.TenantSN,
                TenantName = tenantName,
                TimeZoneNum = timeZone.TimeZoneNum,
                TimeZoneDesc = timeZone.TimeZoneDesc,
                Remark = timeZone.Remark,
                IsOperationCompany = isOp,
                TenantOptions = tenantOptions
            };
        }

        public async Task<(bool Success, string Message)> CreateAsync(TimeZoneEditViewModel model, string currentUser)
        {
            // 檢查區段編碼是否重複
            var exists = await _repo.CheckTimeZoneNumExistsAsync(model.TimeZoneNum, model.TenantSN);
            if (exists)
            {
                return (false, "此區段編碼已存在，請使用其他編碼。");
            }

            var entity = new ADAMS.Models.TimeZone
            {
                TenantSN = model.TenantSN,
                TimeZoneNum = model.TimeZoneNum,
                TimeZoneDesc = model.TimeZoneDesc,
                Remark = model.Remark,
                IsDeleted = false,
                CreateTime = DateTime.Now,
                CreateUser = currentUser
            };

            await _repo.AddAsync(entity);
            return (true, "時間區段已新增。");
        }

        public async Task<(bool Success, string Message)> UpdateAsync(TimeZoneEditViewModel model, string currentUser)
        {
            var entity = await _repo.GetByIdAsync(model.TimeZoneSN!.Value);
            if (entity == null)
            {
                return (false, "找不到指定的時間區段資料。");
            }

            // 檢查區段編碼是否重複（排除自己）
            var exists = await _repo.CheckTimeZoneNumExistsAsync(
                model.TimeZoneNum,
                model.TenantSN,
                model.TimeZoneSN.Value);
            if (exists)
            {
                return (false, "此區段編碼已存在，請使用其他編碼。");
            }

            entity.TimeZoneNum = model.TimeZoneNum;
            entity.TimeZoneDesc = model.TimeZoneDesc;
            entity.Remark = model.Remark;
            entity.ModifyTime = DateTime.Now;
            entity.ModifyUser = currentUser;

            await _repo.UpdateAsync(entity);
            return (true, "時間區段已更新。");
        }

        public async Task SoftDeleteAsync(int timeZoneSN, string currentUser)
        {
            await _repo.SoftDeleteAsync(timeZoneSN, currentUser);
        }
    }
}

