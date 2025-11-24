using System.ComponentModel.DataAnnotations;
using ADAMS.Areas.Models;

namespace ADAMS.Areas.BasicDataManagement.ViewModels.SupplierDataManagement
{
    // 列表單筆
    public class SupplierListItemViewModel
    {
        public int SupplierSN { get; set; }
        public string SupplierNum { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public string? ContactName { get; set; }
        public string? ContactPhone { get; set; }
        public int SupplierType { get; set; }
        public string SupplierTypeName { get; set; } = string.Empty;
        public string? Remark { get; set; }
    }

    // 列表頁
    public class SupplierListViewModel
    {
        public bool IsOperationCompany { get; set; }
        public int CurrentTenantSN { get; set; }

        public List<TenantOption> TenantOptions { get; set; } = new();

        // 查詢條件
        public string? Keyword { get; set; }
        public int SupplierType { get; set; } // 0=全部, 1=飼料, 2=魚苗, 3=其他

        public List<SupplierListItemViewModel> Suppliers { get; set; } = new();

        public PaginationInfo Pagination { get; set; } = new();
    }

    // 編輯頁
    public class SupplierEditViewModel
    {
        public int? SupplierSN { get; set; }

        [Required]
        public int TenantSN { get; set; }

        [Display(Name = "養殖戶")]
        public string TenantName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "廠商代號")]
        [StringLength(20)]
        public string SupplierNum { get; set; } = string.Empty;

        [Required]
        [Display(Name = "廠商名稱")]
        [StringLength(50)]
        public string SupplierName { get; set; } = string.Empty;

        [Display(Name = "聯絡人姓名")]
        [StringLength(50)]
        public string? ContactName { get; set; }

        [Display(Name = "聯絡人電話")]
        [StringLength(50)]
        public string? ContactPhone { get; set; }

        [Required]
        [Display(Name = "營業型態")]
        public int SupplierType { get; set; } // 1=飼料,2=魚苗,3=其他

        [Display(Name = "備註")]
        [StringLength(100)]
        public string? Remark { get; set; }

        // for 畫面：判斷是否營運公司
        public bool IsOperationCompany { get; set; }
        public List<TenantOption> TenantOptions { get; set; } = new();
    }
}
