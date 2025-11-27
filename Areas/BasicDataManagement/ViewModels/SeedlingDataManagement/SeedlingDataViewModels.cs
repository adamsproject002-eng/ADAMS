using ADAMS.Areas.Models;
using System.ComponentModel.DataAnnotations;

namespace ADAMS.Areas.BasicDataManagement.ViewModels.SeedlingDataManagement
{
    public class FishVarietyOption
    {
        public int FVSN { get; set; }
        public string FVName { get; set; } = "";
    }

    public class SupplierOption
    {
        public int SupplierSN { get; set; }
        public string SupplierName { get; set; } = "";
    }

    /// <summary>列表上的每一列</summary>
    public class SeedlingListItemViewModel
    {
        public int FrySN { get; set; }
        public string FryName { get; set; } = "";
        public string SupplierName { get; set; } = "";
        public string FishVarietyName { get; set; } = "";
        public string? UnitName { get; set; }
        public string? Remark { get; set; }
    }

    /// <summary>列表頁 ViewModel</summary>
    public class SeedlingListViewModel
    {
        public bool IsOperationCompany { get; set; }
        public int CurrentTenantSN { get; set; }

        public List<TenantOption> TenantOptions { get; set; } = new();
        public List<FishVarietyOption> FishVarietyOptions { get; set; } = new();

        public int SelectedFVSN { get; set; }          // 0 = 全部
        public string? Keyword { get; set; }

        public List<SeedlingListItemViewModel> Items { get; set; } = new();

        public PaginationInfo Pagination { get; set; } = new PaginationInfo();
    }

    /// <summary>新增/修改用 ViewModel</summary>
    public class SeedlingEditViewModel
    {
        public int? FrySN { get; set; }

        [Required]
        public int TenantSN { get; set; }

        public string TenantName { get; set; } = "";

        [Required]
        [Display(Name = "魚苗名稱")]
        [StringLength(50)]
        public string FryName { get; set; } = "";

        [Required]
        [Display(Name = "供應商")]
        public int SupplierSN { get; set; }

        [Required]
        [Display(Name = "魚種")]
        public int FVSN { get; set; }

        [Display(Name = "單位")]
        [StringLength(50)]
        public string? UnitName { get; set; }

        [Display(Name = "備註")]
        [StringLength(100)]
        public string? Remark { get; set; }

        public bool IsOperationCompany { get; set; }

        public List<TenantOption> TenantOptions { get; set; } = new();
        public List<SupplierOption> SupplierOptions { get; set; } = new();
        public List<FishVarietyOption> FishVarietyOptions { get; set; } = new();
    }
}
