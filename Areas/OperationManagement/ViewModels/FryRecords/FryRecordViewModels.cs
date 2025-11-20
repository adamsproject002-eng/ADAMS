using System.ComponentModel.DataAnnotations;

namespace ADAMS.Areas.OperationManagement.ViewModels.FryRecords
{
    // ==================== 列表頁 ViewModel ====================
    public class FryRecordListViewModel
    {
        public List<FryRecordListItemViewModel> Records { get; set; } = new();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }

        // 查詢條件回傳
        public int TenantSN { get; set; }
        public int? AreaSN { get; set; }
        public int? PondSN { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }

    // ==================== 列表項目 ====================
    public class FryRecordListItemViewModel
    {
        public int FryRecordSN { get; set; }

        [Display(Name = "養殖戶")]
        public string TenantName { get; set; } = string.Empty;

        [Display(Name = "場區")]
        public string AreaName { get; set; } = string.Empty;

        [Display(Name = "魚池")]
        public string PondName { get; set; } = string.Empty;

        [Display(Name = "放養次數")]
        public int FarmingNum { get; set; }

        [Display(Name = "放養碼")]
        public string FarmingCode { get; set; } = string.Empty;

        [Display(Name = "放苗日期")]
        public DateTime FarmingDate { get; set; }

        [Display(Name = "放苗尾數")]
        public int FarmingPCS { get; set; }

        [Display(Name = "苗齡(克/尾)")]
        public decimal FryAge { get; set; }

        [Display(Name = "水面積")]
        public decimal PondArea { get; set; }

        [Display(Name = "放養密度")]
        public decimal FarmingDensity { get; set; }

        [Display(Name = "管理人")]
        public string ManageAccount { get; set; } = string.Empty;

        [Display(Name = "備註")]
        public string? Remark { get; set; }

        [Display(Name = "狀態")]
        public bool IsDeleted { get; set; }
    }

    // ==================== 詳細頁 ====================
    public class FryRecordDetailViewModel
    {
        public int FryRecordSN { get; set; }

        [Display(Name = "養殖戶")]
        public string TenantName { get; set; } = string.Empty;

        [Display(Name = "場區")]
        public string AreaName { get; set; } = string.Empty;

        [Display(Name = "魚池")]
        public string PondName { get; set; } = string.Empty;

        [Display(Name = "放養次數")]
        public int FarmingNum { get; set; }

        [Display(Name = "放養碼")]
        public string FarmingCode { get; set; } = string.Empty;

        [Display(Name = "放苗日期")]
        public DateTime FarmingDate { get; set; }

        [Display(Name = "放苗尾數")]
        public int FarmingPCS { get; set; }

        [Display(Name = "苗齡(克/尾)")]
        public decimal FryAge { get; set; }

        [Display(Name = "水面積")]
        public decimal PondArea { get; set; }

        [Display(Name = "放養密度")]
        public decimal FarmingDensity { get; set; }

        [Display(Name = "管理人")]
        public string ManageAccount { get; set; } = string.Empty;

        [Display(Name = "備註")]
        public string? Remark { get; set; }

        [Display(Name = "建立時間")]
        public DateTime CreateTime { get; set; }

        [Display(Name = "建立者")]
        public string CreateUser { get; set; } = string.Empty;

        [Display(Name = "最後修改時間")]
        public DateTime? ModifyTime { get; set; }

        [Display(Name = "最後修改者")]
        public string? ModifyUser { get; set; }
    }

    // ==================== 新增 ====================
    public class FryRecordCreateViewModel
    {
        [Required]
        [Display(Name = "魚池")]
        public int PondSN { get; set; }

        [Required]
        [Display(Name = "放養次數")]
        public int FarmingNum { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "放養碼")]
        public string FarmingCode { get; set; } = string.Empty;

        [Required]
        [Display(Name = "放苗日期")]
        public DateTime FarmingDate { get; set; }

        [Required]
        [Display(Name = "放苗尾數")]
        public int FarmingPCS { get; set; }

        [Required]
        [Display(Name = "苗齡(克/尾)")]
        public decimal FryAge { get; set; }

        [Required]
        [Display(Name = "水面積")]
        public decimal PondArea { get; set; }

        [Required]
        [Display(Name = "放養密度")]
        public decimal FarmingDensity { get; set; }

        [Required]
        [Display(Name = "管理人")]
        public string ManageAccount { get; set; } = string.Empty;

        [Display(Name = "備註")]
        public string? Remark { get; set; }
    }

    // ==================== 編輯 ====================
    public class FryRecordEditViewModel : FryRecordCreateViewModel
    {
        public int FryRecordSN { get; set; }
    }

    // ==================== 下拉選單 ====================
    public class FryRecordDropdownDto
    {
        public int Value { get; set; }
        public string Label { get; set; } = string.Empty;
    }
}
