using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ADAMS.Areas.Models;

namespace ADAMS.Areas.BasicDataManagement.ViewModels.UnitDataManagement
{
    /// <summary>
    /// 清單每一列
    /// </summary>
    public class UnitDataListItemViewModel
    {
        public int UnitSN { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public string? Remark { get; set; }
    }

    /// <summary>
    /// 清單頁 ViewModel
    /// </summary>
    public class UnitDataListViewModel
    {
        public bool IsOperationCompany { get; set; }
        public string? Keyword { get; set; }

        public List<UnitDataListItemViewModel> Items { get; set; } = new();

        public PaginationInfo? Pagination { get; set; }
    }

    /// <summary>
    /// 新增/修改用 ViewModel
    /// </summary>
    public class UnitDataEditViewModel
    {
        public int? UnitSN { get; set; }  // null = 新增, 有值 = 修改

        [Display(Name = "單位名稱")]
        [Required(ErrorMessage = "{0}為必填")]
        [StringLength(50, ErrorMessage = "{0}長度不可超過 {1} 個字元")]
        public string UnitName { get; set; } = string.Empty;

        [Display(Name = "備註")]
        [StringLength(100, ErrorMessage = "{0}長度不可超過 {1} 個字元")]
        public string? Remark { get; set; }

        public bool IsOperationCompany { get; set; }
    }
}
