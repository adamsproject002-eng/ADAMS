using ADAMS.Areas.Models;

namespace ADAMS.Areas.PondOverview.ViewModels.PondOverviewPage
{
    /// <summary>
    /// 養殖池總覽畫面 ViewModel
    /// </summary>
    public class PondOverviewListViewModel
    {
        public bool IsOperationCompany { get; set; }

        public int CurrentTenantSN { get; set; }
        public List<TenantOption> TenantOptions { get; set; } = new();

        public int CurrentAreaSN { get; set; }
        public List<AreaOption> AreaOptions { get; set; } = new();

        public string CurrentAreaName { get; set; } = string.Empty;
        public decimal CurrentAreaTotalArea { get; set; }

        public List<PondCardViewModel> Ponds { get; set; } = new();
    }

    /// <summary>
    /// 總覽畫面一個卡片要顯示的資訊
    /// </summary>
    public class PondCardViewModel
    {
        public int PondSN { get; set; }
        public string PondNum { get; set; } = string.Empty;
        public decimal PondArea { get; set; }

        public int StockingQty { get; set; }          // 放養尾數（目前先預留）
        public string FishVarietyName { get; set; } = string.Empty; // 品種名稱
        public decimal CurrentABW { get; set; }       // 目前 ABW
    }

    /// <summary>
    /// 養殖池詳細資料畫面 ViewModel
    /// </summary>
    public class PondDetailViewModel
    {
        public int PondSN { get; set; }
        public string PondNum { get; set; } = string.Empty;
        public string AreaName { get; set; } = string.Empty;
        public string TenantName { get; set; } = string.Empty;

        public decimal PondArea { get; set; }

        public string ManagerName { get; set; } = string.Empty;      // 當前管理員
        public string StockingCode { get; set; } = string.Empty;     // 放養碼
        public string FishVarietyName { get; set; } = string.Empty;  // 放養種類
        public string PLorGramUnit { get; set; } = string.Empty;     // PL 或 克/尾
        public int StockingQty { get; set; }                          // 放養總尾數
        public decimal Density { get; set; }                          // 密度 pcs/m2

        public DateTime? StockingDate { get; set; }                  // 放苗日
        public DateTime? LastSampleOrHarvestDate { get; set; }       // 最近收成/採樣日
        public decimal CurrentABW { get; set; }                       // 最近 ABW
        public int DOC { get; set; }                                  // DOC（養殖天數）

        public decimal AccumulatedHarvestWeight { get; set; }        // 累計收成重量
        public int HarvestTimes { get; set; }                         // 收穫次數
        public decimal SurvivalRate { get; set; }                     // 存活率 %

        public string? Remark { get; set; }
    }
}
