using ADAMS.Areas.BasicDataManagement.Repositories.GrowthTargetReference;
using ADAMS.Areas.BasicDataManagement.ViewModels.GrowthTargetReference;
using ADAMS.Models;
using ADAMS.Services;

namespace ADAMS.Areas.BasicDataManagement.Services.GrowthTargetReference
{
    public class GrowthTargetService : IGrowthTargetService
    {
        private readonly IGrowthTargetRepository _repo;
        private readonly ICurrentAccountService _current;

        public GrowthTargetService(
            IGrowthTargetRepository repo,
            ICurrentAccountService current)
        {
            _repo = repo;
            _current = current;
        }

        public async Task<GrowthTargetListViewModel> GetListViewModelAsync(int? fvsn)
        {
            var varieties = await _repo.GetFishVarietiesAsync();
            if (!varieties.Any())
            {
                return new GrowthTargetListViewModel
                {
                    IsOperationCompany = _current.IsOperationCompany
                };
            }

            var selectedFvsn = fvsn ?? varieties.First().FVSN;
            var selectedVariety = varieties.FirstOrDefault(v => v.FVSN == selectedFvsn) ?? varieties.First();

            var main = await _repo.GetMainByFVSNAsync(selectedVariety.FVSN);

            var items = new List<GrowthTargetListItemViewModel>();
            if (main != null)
            {
                var details = await _repo.GetDetailsAsync(main.GTMSN);
                items = details.Select(d => new GrowthTargetListItemViewModel
                {
                    GTDSN = d.GTDSN,
                    DOC = d.GT_DOC,
                    ABW = d.GT_ABW,
                    DailyGrow = d.GT_DailyGrow,
                    Remark = d.Remark
                }).ToList();
            }

            return new GrowthTargetListViewModel
            {
                IsOperationCompany = _current.IsOperationCompany,
                SelectedFVSN = selectedVariety.FVSN,
                SelectedFishVarietyName = selectedVariety.FVName,
                FishVarietyOptions = varieties
                    .Select(v => new FishVarietyOption { FVSN = v.FVSN, FVName = v.FVName })
                    .ToList(),
                Items = items
            };
        }

        public async Task<GrowthTargetEditViewModel> GetCreateViewModelAsync(int fvsn)
        {
            var listVm = await GetListViewModelAsync(fvsn);
            var variety = listVm.FishVarietyOptions.First(v => v.FVSN == listVm.SelectedFVSN);

            // 先找 main，有就用，沒有就用 0，等存檔時再建立
            var main = await _repo.GetMainByFVSNAsync(variety.FVSN);

            return new GrowthTargetEditViewModel
            {
                GTDSN = null,
                GTMSN = main?.GTMSN ?? 0,
                FVSN = variety.FVSN,
                FishVarietyName = variety.FVName,
                IsOperationCompany = _current.IsOperationCompany
            };
        }

        public async Task<GrowthTargetEditViewModel?> GetEditViewModelAsync(int gtdsn)
        {
            var detail = await _repo.GetDetailAsync(gtdsn);
            if (detail == null) return null;

            var main = detail.Main!;
            var fv = main.FishVariety!;

            return new GrowthTargetEditViewModel
            {
                GTDSN = detail.GTDSN,
                GTMSN = main.GTMSN,
                FVSN = fv.FVSN,
                FishVarietyName = fv.FVName,
                //GTMName = main!.GTMName,
                DOC = detail.GT_DOC,
                ABW = detail.GT_ABW,
                DailyGrow = detail.GT_DailyGrow,
                Remark = detail.Remark,
                IsOperationCompany = _current.IsOperationCompany
            };
        }

        public async Task CreateAsync(GrowthTargetEditViewModel model)
        {
            EnsureOperationCompany();

            // 先取得主表，沒有就建一筆
            var main = await _repo.GetMainByFVSNAsync(model.FVSN);
            if (main == null)
            {
                var gtmName = $"{model.FishVarietyName}成長目標";
                main = await _repo.CreateMainAsync(model.FVSN, gtmName, _current.Name);
            }
            // 更新主表成長目標名稱（暫時註解掉，避免不必要的修改時間更新）
            //if (!string.Equals(main.GTMName, model.GTMName, StringComparison.Ordinal))
            //{
            //    main.GTMName = model.GTMName;
            //    main.ModifyTime = DateTime.Now;
            //    main.ModifyUser = _current.Name;
            //    await _repo.UpdateMainAsync(main);
            //}
            var detail = new GrowTargetDetail
            {
                GTMSN = main.GTMSN,
                GT_DOC = model.DOC,
                GT_ABW = model.ABW,
                GT_DailyGrow = 0m, // 先設 0，後面計算
                Remark = model.Remark,
                IsDeleted = false,
                CreateTime = DateTime.Now,
                CreateUser = _current.Name
            };

            // 算 DailyGrow
            await CalcDailyGrowAsync(detail);

            await _repo.AddDetailAsync(detail);
        }

        public async Task UpdateAsync(GrowthTargetEditViewModel model)
        {
            EnsureOperationCompany();

            var entity = await _repo.GetDetailAsync(model.GTDSN!.Value);
            if (entity == null) return;

            entity.GT_DOC = model.DOC;
            entity.GT_ABW = model.ABW;
            entity.Remark = model.Remark;
            entity.ModifyTime = DateTime.Now;
            entity.ModifyUser = _current.Name;

            await CalcDailyGrowAsync(entity);

            await _repo.UpdateDetailAsync(entity);
        }

        public async Task SoftDeleteAsync(int gtdsn)
        {
            EnsureOperationCompany();
            await _repo.SoftDeleteDetailAsync(gtdsn, _current.Name);
        }

        /// <summary>
        /// DailyGrow = (本筆 ABW - 前一筆 ABW) / (本筆 DOC - 前一筆 DOC)
        /// 第一筆（最小 DOC）DailyGrow = 0
        /// </summary>
        private async Task CalcDailyGrowAsync(GrowTargetDetail current)
        {
            // 把同主表的所有明細抓出來（含本筆，尚未存檔時可能還不在 DB）
            var all = await _repo.GetDetailsAsync(current.GTMSN);
            //過濾掉原本舊的那一筆
            all = all.Where(d => d.GTDSN != current.GTDSN).ToList();
            //為了計算新的值，把 current 加進去
            all.Add(current);

            var ordered = all.OrderBy(d => d.GT_DOC).ToList();

            GrowTargetDetail? prev = null;
            foreach (var d in ordered)
            {
                if (prev == null)
                {
                    d.GT_DailyGrow = 0m;
                }
                else
                {
                    var days = d.GT_DOC - prev.GT_DOC;
                    if (days <= 0)
                    {
                        d.GT_DailyGrow = 0m;
                    }
                    else
                    {
                        var grow = d.GT_ABW - prev.GT_ABW;
                        d.GT_DailyGrow = Math.Round(grow / days, 2, MidpointRounding.AwayFromZero);
                    }
                }

                prev = d;
            }

            // 把 current 的 DailyGrow 更新回來
            current.GT_DailyGrow = ordered
                .First(d => d.GT_DOC == current.GT_DOC && d.GT_ABW == current.GT_ABW)
                .GT_DailyGrow;
        }

        private void EnsureOperationCompany()
        {
            if (!_current.IsOperationCompany)
            {
                throw new InvalidOperationException("只有營運公司帳號可以維護成長目標參考資料。");
            }
        }
    }
}
