using ADAMS.Models;

namespace ADAMS.Areas.BasicDataManagement.ViewModels.FeedingPlanManagement
{
    public class TenantOption
    {
        public int SN { get; set; }
        public string TenantName { get; set; } = string.Empty;
    }

    public class FishVarietyOption
    {
        public int FVSN { get; set; }
        public string FVName { get; set; } = string.Empty;
    }

    public class FeedingPlanListItemViewModel
    {
        public int FPDSN { get; set; }
        public int DOC { get; set; }
        public decimal ABW { get; set; }
        public decimal FeedingRate { get; set; }
        public string? Remark { get; set; }
    }

    public class FeedingPlanListViewModel
    {
        public bool IsOperationCompany { get; set; }

        public int CurrentTenantSN { get; set; }
        public string CurrentTenantName { get; set; } = string.Empty;

        public int SelectedFVSN { get; set; }
        public string SelectedFishVarietyName { get; set; } = string.Empty;

        public List<TenantOption> TenantOptions { get; set; } = new();
        public List<FishVarietyOption> FishVarietyOptions { get; set; } = new();
        public List<FeedingPlanListItemViewModel> Items { get; set; } = new();
    }

    public class FeedingPlanEditViewModel
    {
        public int? FPDSN { get; set; }
        public int FPMSN { get; set; }

        public int TenantSN { get; set; }
        public string TenantName { get; set; } = string.Empty;

        public int FVSN { get; set; }
        public string FishVarietyName { get; set; } = string.Empty;

        public int DOC { get; set; }
        public decimal ABW { get; set; }
        public decimal FeedingRate { get; set; }
        public string? Remark { get; set; }

        public bool IsOperationCompany { get; set; }
    }
}
