using System.ComponentModel.DataAnnotations;

namespace ADAMS.Areas.BasicDataManagement.ViewModels.GrowthTargetReference
{
    public class FishVarietyOption
    {
        public int FVSN { get; set; }
        public string FVName { get; set; } = "";
    }

    public class GrowthTargetListItemViewModel
    {
        public int GTDSN { get; set; }
        public int DOC { get; set; }
        public decimal ABW { get; set; }
        public decimal DailyGrow { get; set; }
        public string? Remark { get; set; }
    }

    public class GrowthTargetListViewModel
    {
        public bool IsOperationCompany { get; set; }

        public int SelectedFVSN { get; set; }
        public string SelectedFishVarietyName { get; set; } = "";

        public List<FishVarietyOption> FishVarietyOptions { get; set; } = new();

        public List<GrowthTargetListItemViewModel> Items { get; set; } = new();
    }

    public class GrowthTargetEditViewModel
    {
        public int? GTDSN { get; set; }
        public int GTMSN { get; set; }

        [Required]
        public int FVSN { get; set; }

        public string FishVarietyName { get; set; } = "";

        //[Required]
        //[Display(Name = "成長目標名稱")]
        //[StringLength(50)]
        //public string GTMName { get; set; } = string.Empty; 

        [Display(Name = "DOC(days)")]
        [Required]
        public int DOC { get; set; }

        [Display(Name = "ABW(g/pc)")]
        [Required]
        public decimal ABW { get; set; }

        [Display(Name = "DailyGrow(g/pc/day)")]
        public decimal DailyGrow { get; set; }

        [Display(Name = "備註")]
        [StringLength(100)]
        public string? Remark { get; set; }

        public bool IsOperationCompany { get; set; }
    }
}
