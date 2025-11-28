using System.ComponentModel.DataAnnotations;

namespace ADAMS.Areas.BasicDataManagement.ViewModels.FarmingTypeManagement
{
    /// <summary>
    /// 清單每一列
    /// </summary>
    public class FarmingTypeListItemViewModel
    {
        public int FVSN { get; set; }
        public string FVName { get; set; } = string.Empty;
        public string? Remark { get; set; }
    }

    /// <summary>
    /// 清單頁 ViewModel
    /// </summary>
    public class FarmingTypeListViewModel
    {
        public bool IsOperationCompany { get; set; }
        public string? Keyword { get; set; }

        public List<FarmingTypeListItemViewModel> Items { get; set; } = new();
    }

    /// <summary>
    /// 新增/修改用 ViewModel
    /// </summary>
    public class FarmingTypeEditViewModel
    {
        public int? FVSN { get; set; }  // null = 新增, 有值 = 修改

        [Display(Name = "魚種名稱")]
        [Required(ErrorMessage = "{0}為必填")]
        [StringLength(20, ErrorMessage = "{0}長度不可超過 {1} 個字元")]
        public string FVName { get; set; } = string.Empty;

        [Display(Name = "備註")]
        [StringLength(100, ErrorMessage = "{0}長度不可超過 {1} 個字元")]
        public string? Remark { get; set; }

        public bool IsOperationCompany { get; set; }
    }
}
