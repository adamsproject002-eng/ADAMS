using ADAMS.Areas.Models;
using ADAMS.Areas.PondOverview.ViewModels.PondOverviewPage;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace ADAMS.Areas.OperationManagement.ViewModels.FeedingRecordQuery
{
    public class FeedingRecordListItemViewModel
    {
        public int FeedingRecordSN { get; set; }
        public string FarmingCode { get; set; } = string.Empty;
        public DateTime FeedingDate { get; set; }
        public string TimeZoneText { get; set; } = string.Empty;
        public string FeedBrand { get; set; } = string.Empty;
        public string FeedName { get; set; } = string.Empty;
        public decimal FeedingAmount { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal? SurvivalRate { get; set; }
        public decimal ABW { get; set; }
        public int DOC { get; set; }
        public decimal? ABWGuide { get; set; } // AWB 基準（保留欄位）
    }

    public class FeedingRecordListViewModel
    {
        public bool IsOperationCompany { get; set; }

        // 篩選條件
        public int CurrentTenantSN { get; set; }
        public int? CurrentAreaSN { get; set; }
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        public List<TenantOption> TenantOptions { get; set; } = new();
        public List<AreaOption> AreaOptions { get; set; } = new();

        public List<FeedingRecordListItemViewModel> Items { get; set; } = new();
    }

    public class FeedingRecordEditViewModel
    {
        public int? FeedingRecordSN { get; set; }

        public bool IsOperationCompany { get; set; }

        public int TenantSN { get; set; }
        public List<TenantOption> TenantOptions { get; set; } = new();

        public int AreaSN { get; set; }
        public List<AreaOption> AreaOptions { get; set; } = new();

        public int PondSN { get; set; }
        public List<PondOption> PondOptions { get; set; } = new();

        [Display(Name = "放養碼")]
        public string FarmingCode { get; set; } = "";

        [Display(Name = "投料日期")]
        [DataType(DataType.Date)]
        public DateTime FeedingDate { get; set; }

        [Display(Name = "投料時間")]
        public int TimeZoneSN { get; set; }
        public List<TimeZoneOptionVM> TimeZoneOptions { get; set; } = new();

        [Display(Name = "飼料")]
        public int FeedSN { get; set; }
        public List<FeedOption> FeedOptions { get; set; } = new();

        [Display(Name = "投料數量 (Kgs)")]
        public decimal FeedingAmount { get; set; }

        [Display(Name = "單位")]
        public string Unit { get; set; } = "公斤";

        [Display(Name = "估計存活率")]
        public decimal? SurvivalRate { get; set; }

        [Display(Name = "ABW")]
        public decimal ABW { get; set; }

        [Display(Name = "DOC")]
        public int DOC { get; set; }

        [Display(Name = "管理人員")]
        public string ManageAccount { get; set; } = "";
        public List<ManageAccountOptionVM> ManageAccountOptions { get; set; } = new();

        // 顯示用資訊（魚池、魚種等）
        public string AreaName { get; set; } = "";
        public string PondNum { get; set; } = "";
        [Display(Name = "養殖種類")]
        public string FishVarietyName { get; set; } = "";

        // 估算採用基準（保留欄位）
        public decimal? AWBGuide { get; set; }
        public int? StockingQty { get; set; }
        public decimal? SurvivalBase { get; set; }
        public int SupplierSN { get; set; }
        public List<SupplierOption> SupplierOptions { get; set; } = new();
    }

    public class ManageAccountOptionVM
    {
        public string AccountName { get; set; } = "";
        public string RealName { get; set; } = "";
    }
}
