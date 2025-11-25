using System.ComponentModel.DataAnnotations;
using ADAMS.Areas.Models;

namespace ADAMS.Areas.BasicDataManagement.ViewModels.TimeSegmentSetting
{
    // 列表單筆
    public class TimeZoneListItemViewModel
    {
        public int TimeZoneSN { get; set; }
        public string TimeZoneNum { get; set; } = string.Empty;
        public string TimeZoneDesc { get; set; } = string.Empty;
        public string? Remark { get; set; }
    }

    // 列表頁
    public class TimeZoneListViewModel
    {
        public bool IsOperationCompany { get; set; }
        public int CurrentTenantSN { get; set; }

        public List<TenantOption> TenantOptions { get; set; } = new();

        // 查詢條件
        public string? Keyword { get; set; }

        public List<TimeZoneListItemViewModel> TimeZones { get; set; } = new();

        public PaginationInfo Pagination { get; set; } = new();
    }

    // 編輯頁
    public class TimeZoneEditViewModel
    {
        public int? TimeZoneSN { get; set; }

        [Required]
        public int TenantSN { get; set; }

        [Display(Name = "養殖戶")]
        public string TenantName { get; set; } = string.Empty;

        [Required(ErrorMessage = "區段編碼為必填")]
        [Display(Name = "區段編碼")]
        [StringLength(20, ErrorMessage = "區段編碼最多 20 字")]
        public string TimeZoneNum { get; set; } = string.Empty;

        [Required(ErrorMessage = "區段描述為必填")]
        [Display(Name = "區段描述")]
        [StringLength(50, ErrorMessage = "區段描述最多 50 字")]
        public string TimeZoneDesc { get; set; } = string.Empty;

        [Display(Name = "備註")]
        [StringLength(100, ErrorMessage = "備註最多 100 字")]
        public string? Remark { get; set; }

        // for 畫面：判斷是否營運公司
        public bool IsOperationCompany { get; set; }
        public List<TenantOption> TenantOptions { get; set; } = new();
    }
}
