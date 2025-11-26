using ADAMS.Areas.Models;
using System.ComponentModel.DataAnnotations;

namespace ADAMS.Areas.BasicDataManagement.ViewModels.AreaPondManagement
{
    // ---------- 場區清單 ----------

    public class AreaListItemViewModel
    {
        public int AreaSN { get; set; }
        public string AreaName { get; set; } = string.Empty;
        public string? GPSX { get; set; }
        public string? GPSY { get; set; }

        public int PondCount { get; set; }          // 該場區池數量
        public decimal TotalPondArea { get; set; }  // 該場區總面積(m2)

        public string? Remark { get; set; }
    }

    public class AreaListViewModel
    {
        public bool IsOperationCompany { get; set; }
        public int CurrentTenantSN { get; set; }

        public List<TenantOption> TenantOptions { get; set; } = new();

        public string? Keyword { get; set; }

        public List<AreaListItemViewModel> Areas { get; set; } = new();

        public PaginationInfo Pagination { get; set; } = new();
    }

    // ---------- 場區新增 / 修改 ----------

    public class AreaEditViewModel
    {
        public int? AreaSN { get; set; } // null = 新增

        [Required]
        public int TenantSN { get; set; }

        public string TenantName { get; set; } = string.Empty; // 顯示用

        [Required]
        [Display(Name = "場區名稱")]
        [StringLength(50)]
        public string AreaName { get; set; } = string.Empty;

        [Display(Name = "GPS 座標 X")]
        [StringLength(20)]
        public string? GPSX { get; set; }

        [Display(Name = "GPS 座標 Y")]
        [StringLength(20)]
        public string? GPSY { get; set; }

        [Display(Name = "備註")]
        [StringLength(100)]
        public string? Remark { get; set; }

        public bool IsOperationCompany { get; set; }
        public List<TenantOption> TenantOptions { get; set; } = new();
    }

    // ---------- 養殖池清單 ----------

    public class PondListItemViewModel
    {
        public int PondSN { get; set; }
        public string PondNum { get; set; } = string.Empty;
        public decimal PondWidth { get; set; }
        public decimal PondLength { get; set; }
        public decimal PondArea { get; set; }
        public string? Remark { get; set; }
    }

    public class PondListViewModel
    {
        public int AreaSN { get; set; }
        public string AreaName { get; set; } = string.Empty;
        public decimal TotalPondArea { get; set; }

        public string? Keyword { get; set; }

        public List<PondListItemViewModel> Ponds { get; set; } = new();

        public PaginationInfo Pagination { get; set; } = new();
    }

    // ---------- 養殖池新增 / 修改 ----------

    public class PondEditViewModel
    {
        public int? PondSN { get; set; } // null = 新增

        [Required]
        public int AreaSN { get; set; }

        public string AreaName { get; set; } = string.Empty; // 顯示用

        [Required]
        public int TenantSN { get; set; }

        [Display(Name = "池號")]
        [Required]
        [StringLength(20)]
        public string PondNum { get; set; } = string.Empty;

        [Display(Name = "最寬邊(m)")]
        public decimal PondWidth { get; set; }

        [Display(Name = "長 / 直徑(m)")]
        public decimal PondLength { get; set; }

        [Display(Name = "水面積(m2)")]
        public decimal PondArea { get; set; }

        [Display(Name = "GPS 座標 X")]
        [StringLength(20)]
        public string? GPSX { get; set; }

        [Display(Name = "GPS 座標 Y")]
        [StringLength(20)]
        public string? GPSY { get; set; }

        [Display(Name = "備註")]
        [StringLength(100)]
        public string? Remark { get; set; }
    }
}
