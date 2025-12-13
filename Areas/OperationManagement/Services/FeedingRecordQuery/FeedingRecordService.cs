// Areas/OperationManagement/Services/FeedingRecordQuery/FeedingRecordService.cs
using ADAMS.Areas.Models;
using ADAMS.Areas.OperationManagement.Repositories.FeedingRecordQuery;
using ADAMS.Areas.OperationManagement.ViewModels.FeedingRecordQuery;
using ADAMS.Models;
using ADAMS.Services;
using ClosedXML.Excel;
using System.Globalization;
using System.Text;

namespace ADAMS.Areas.OperationManagement.Services.FeedingRecordQuery
{
    public class FeedingRecordService : IFeedingRecordService
    {
        private readonly IFeedingRecordRepository _repo;
        private readonly ICurrentAccountService _current;

        public FeedingRecordService(
            IFeedingRecordRepository repo,
            ICurrentAccountService current)
        {
            _repo = repo;
            _current = current;
        }

        public async Task<FeedingRecordListViewModel> GetListViewModelAsync(
            int? tenantSN,
            int? areaSN,
            DateTime? startDate,
            DateTime? endDate)
        {
            var isOp = _current.IsOperationCompany;
            var currentTenantSN = isOp ? (tenantSN ?? _current.TenantSN) : _current.TenantSN;

            var tenantOptions = await _repo.GetTenantOptionsAsync(isOp, currentTenantSN);

            if (!tenantOptions.Any())
            {
                return new FeedingRecordListViewModel
                {
                    IsOperationCompany = isOp
                };
            }

            if (!tenantOptions.Any(t => t.SN == currentTenantSN))
            {
                currentTenantSN = tenantOptions.First().SN;
            }

            var areaOptions = await _repo.GetAreaOptionsAsync(currentTenantSN);

            int? currentAreaSN = areaSN;
            if (currentAreaSN.HasValue && !areaOptions.Any(a => a.AreaSN == currentAreaSN.Value))
            {
                currentAreaSN = areaOptions.FirstOrDefault()?.AreaSN;
            }

            var items = await _repo.SearchAsync(currentTenantSN, currentAreaSN, startDate, endDate);

            return new FeedingRecordListViewModel
            {
                IsOperationCompany = isOp,
                CurrentTenantSN = currentTenantSN,
                CurrentAreaSN = currentAreaSN,
                StartDate = startDate,
                EndDate = endDate,
                TenantOptions = tenantOptions,
                AreaOptions = areaOptions,
                Items = items
            };
        }

        public async Task<FeedingRecordEditViewModel> GetCreateViewModelAsync(
            int? tenantSN,
            int? areaSN,
            int? pondSN)
        {
            var isOp = _current.IsOperationCompany;
            var currentTenantSN = isOp ? (tenantSN ?? _current.TenantSN) : _current.TenantSN;

            var tenantOptions = await _repo.GetTenantOptionsAsync(isOp, currentTenantSN);
            if (!tenantOptions.Any())
            {
                currentTenantSN = _current.TenantSN;
            }

            var areaOptions = await _repo.GetAreaOptionsAsync(currentTenantSN);
            int currentAreaSN = areaSN ?? areaOptions.FirstOrDefault()?.AreaSN ?? 0;

            var pondOptions = await _repo.GetPondOptionsAsync(currentTenantSN, currentAreaSN);
            int currentPondSN = pondSN ?? pondOptions.FirstOrDefault()?.PondSN ?? 0;

            var tzOptions = await _repo.GetTimeZoneOptionsAsync(currentTenantSN);
            var feedOptions = await _repo.GetFeedOptionsAsync(currentTenantSN);
            var manageOptions = await _repo.GetManageAccountOptionsAsync(currentTenantSN);

            var vm = new FeedingRecordEditViewModel
            {
                FeedingRecordSN = null,
                IsOperationCompany = isOp,
                TenantSN = currentTenantSN,
                TenantOptions = tenantOptions,
                AreaSN = currentAreaSN,
                AreaOptions = areaOptions,
                PondSN = currentPondSN,
                PondOptions = pondOptions,
                FeedingDate = DateTime.Today,
                TimeZoneOptions = tzOptions,
                FeedOptions = feedOptions,
                ManageAccountOptions = manageOptions,
                ManageAccount = _current.Name,
                Unit = "公斤"
            };

            // 先帶入該池最近放養資訊（放養碼 / 魚種 / ABW / DOC）
            if (currentPondSN != 0)
            {
                var info = await _repo.GetLatestFarmingInfoAsync(currentPondSN, vm.FeedingDate);
                if (info.HasValue)
                {
                    vm.FarmingCode = info.Value.FarmingCode;
                    vm.FishVarietyName = info.Value.FishVarietyName;
                    vm.ABW = info.Value.ABW;
                    vm.DOC = info.Value.DOC;
                    vm.StockingQty = info.Value.StockingQty;
                }
            }

            vm.SupplierOptions = await _repo.GetFeedSuppliersAsync(vm.TenantSN);

            if (vm.SupplierOptions.Any())
            {
                vm.SupplierSN = vm.SupplierOptions.First().SupplierSN;
                vm.FeedOptions = await _repo.GetFeedsBySupplierAsync(vm.TenantSN, vm.SupplierSN);

                if (vm.FeedOptions.Any())
                {
                    vm.FeedSN = vm.FeedOptions.First().FeedSN;
                }
            }

            return vm;
        }

        public async Task<FeedingRecordEditViewModel?> GetEditViewModelAsync(int id)
        {
            var entity = await _repo.GetAsync(id);
            if (entity == null || entity.Pond == null) return null;

            var tenantSN = entity.Pond.TenantSN;
            var isOp = _current.IsOperationCompany;

            var tenantOptions = await _repo.GetTenantOptionsAsync(isOp, tenantSN);
            var areaOptions = await _repo.GetAreaOptionsAsync(tenantSN);
            var pondOptions = await _repo.GetPondOptionsAsync(tenantSN, entity.Pond.AreaSN);
            var tzOptions = await _repo.GetTimeZoneOptionsAsync(tenantSN);
            var feedOptions = await _repo.GetFeedOptionsAsync(tenantSN);
            var manageOptions = await _repo.GetManageAccountOptionsAsync(tenantSN);



            var vm =  new FeedingRecordEditViewModel
            {
                FeedingRecordSN = entity.FeedingRecordSN,
                IsOperationCompany = isOp,
                TenantSN = tenantSN,
                TenantOptions = tenantOptions,
                AreaSN = entity.Pond.AreaSN,
                AreaOptions = areaOptions,
                PondSN = entity.PondSN,
                PondOptions = pondOptions,
                FarmingCode = entity.FarmingCode,
                FeedingDate = entity.FeedingDate,
                TimeZoneSN = entity.TimeZoneSN,
                TimeZoneOptions = tzOptions,
                FeedSN = entity.FeedSN,
                FeedOptions = feedOptions,
                FeedingAmount = entity.FeedingAmount,
                Unit = entity.Unit,
                SurvivalRate = entity.SurvivalRate,
                ABW = entity.ABW,
                DOC = entity.DOC,
                ManageAccount = entity.ManageAccount,
                ManageAccountOptions = manageOptions,
                AreaName = entity.Pond.Area?.AreaName ?? "",
                PondNum = entity.Pond.PondNum,
                FishVarietyName = "", // 若有 Pond 連到 FishVariety 可在這裡帶入
                AWBGuide = null,
                StockingQty = null,
                SurvivalBase = null,
                SupplierSN = entity.Feed?.SupplierSN ?? 0,
            };

            vm.SupplierOptions = await _repo.GetFeedSuppliersAsync(vm.TenantSN);
            vm.FeedOptions = await _repo.GetFeedsBySupplierAsync(vm.TenantSN, vm.SupplierSN);
            
            return vm;
        }

        public async Task CreateAsync(FeedingRecordEditViewModel model)
        {
            var entity = new FeedingRecord
            {
                PondSN = model.PondSN,
                FarmingCode = model.FarmingCode,
                FeedingDate = model.FeedingDate,
                TimeZoneSN = model.TimeZoneSN,
                FeedSN = model.FeedSN,
                FeedingAmount = model.FeedingAmount,
                Unit = model.Unit,
                SurvivalRate = model.SurvivalRate,
                ABW = model.ABW,
                DOC = model.DOC,
                ManageAccount = model.ManageAccount,
                CreateTime = DateTime.Now,
                CreateUser = _current.Name
            };

            await _repo.AddAsync(entity);
        }

        public async Task UpdateAsync(FeedingRecordEditViewModel model)
        {
            if (!model.FeedingRecordSN.HasValue) return;

            var entity = await _repo.GetAsync(model.FeedingRecordSN.Value);
            if (entity == null) return;

            entity.PondSN = model.PondSN;
            entity.FarmingCode = model.FarmingCode;
            entity.FeedingDate = model.FeedingDate;
            entity.TimeZoneSN = model.TimeZoneSN;
            entity.FeedSN = model.FeedSN;
            entity.FeedingAmount = model.FeedingAmount;
            entity.Unit = model.Unit;
            entity.SurvivalRate = model.SurvivalRate;
            entity.ABW = model.ABW;
            entity.DOC = model.DOC;
            entity.ManageAccount = model.ManageAccount;
            entity.ModifyTime = DateTime.Now;
            entity.ModifyUser = _current.Name;

            await _repo.UpdateAsync(entity);
        }

        public async Task SoftDeleteAsync(int id)
        {
            await _repo.SoftDeleteAsync(id, _current.Name);
        }

        public async Task<byte[]> ExportCsvAsync(int? tenantSN, int? areaSN, DateTime? startDate, DateTime? endDate)
        {
            var isOp = _current.IsOperationCompany;
            var currentTenantSN = isOp ? (tenantSN ?? _current.TenantSN) : _current.TenantSN;

            var items = await _repo.SearchAsync(currentTenantSN, areaSN, startDate, endDate);

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("投餌紀錄");

            // 設定標題列
            worksheet.Cell(1, 1).Value = "放養碼";
            worksheet.Cell(1, 2).Value = "投料日期";
            worksheet.Cell(1, 3).Value = "投料時間";
            worksheet.Cell(1, 4).Value = "品牌";
            worksheet.Cell(1, 5).Value = "名稱";
            worksheet.Cell(1, 6).Value = "kgs";
            worksheet.Cell(1, 7).Value = "估計存活";
            worksheet.Cell(1, 8).Value = "AWB";
            worksheet.Cell(1, 9).Value = "DOC";
            worksheet.Cell(1, 10).Value = "AWB基準";

            // 標題列樣式
            var headerRange = worksheet.Range(1, 1, 1, 10);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

            // 寫入資料
            int row = 2;
            foreach (var x in items)
            {
                worksheet.Cell(row, 1).Value = x.FarmingCode;
                worksheet.Cell(row, 2).Value = x.FeedingDate.ToString("yyyy-MM-dd");
                worksheet.Cell(row, 3).Value = x.TimeZoneText;
                worksheet.Cell(row, 4).Value = x.FeedBrand;
                worksheet.Cell(row, 5).Value = x.FeedName;
                worksheet.Cell(row, 6).Value = x.FeedingAmount;
                worksheet.Cell(row, 7).Value = x.SurvivalRate?.ToString("0.##") ?? "";
                worksheet.Cell(row, 8).Value = x.ABW;
                worksheet.Cell(row, 9).Value = x.DOC;
                worksheet.Cell(row, 10).Value = x.ABWGuide?.ToString("0.##") ?? "";
                row++;
            }

            // 自動調整欄寬
            worksheet.Columns().AdjustToContents();

            // 輸出為 byte[]
            using var memoryStream = new MemoryStream();
            workbook.SaveAs(memoryStream);
            return memoryStream.ToArray();
        }

        public async Task<List<PondOption>> GetPondOptionsAsync(int tenantSN, int areaSN)
        {
            return await _repo.GetPondOptionsAsync(tenantSN, areaSN);
        }
        public async Task<List<FeedOption>> GetFeedOptionsAsync(int tenantSN, int supplierSN)
        {
            return await _repo.GetFeedsBySupplierAsync(tenantSN, supplierSN);
        }
    }
}
