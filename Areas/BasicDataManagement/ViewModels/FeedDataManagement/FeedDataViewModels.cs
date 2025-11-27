using ADAMS.Areas.Models;
using System.ComponentModel.DataAnnotations;

namespace ADAMS.Areas.BasicDataManagement.ViewModels.FeedDataManagement
{
    public class FeedListItemViewModel
    {
        public int FeedSN { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string FeedName { get; set; } = string.Empty;
        public string UnitName { get; set; } = string.Empty;
        public string? Remark { get; set; }
    }

    public class FeedListViewModel
    {
        public bool IsOperationCompany { get; set; }

        public int CurrentTenantSN { get; set; }
        public List<TenantOption> TenantOptions { get; set; } = new();

        public string? Keyword { get; set; }

        public List<FeedListItemViewModel> Feeds { get; set; } = new();

        public PaginationInfo Pagination { get; set; } = new PaginationInfo();
    }

    public class SupplierOption
    {
        public int SupplierSN { get; set; }
        public string SupplierName { get; set; } = string.Empty;
    }

    public class FeedEditViewModel
    {
        public int? FeedSN { get; set; }

        [Required]
        public int TenantSN { get; set; }

        public string TenantName { get; set; } = string.Empty;

        public bool IsOperationCompany { get; set; }

        public List<TenantOption> TenantOptions { get; set; } = new();

        [Required(ErrorMessage = "供應商為必填")]
        [Display(Name = "廠商名稱")]
        public int SupplierSN { get; set; }

        public List<SupplierOption> SupplierOptions { get; set; } = new();

        [Required(ErrorMessage = "飼料名稱為必填")]
        [StringLength(50)]
        [Display(Name = "飼料名稱")]
        public string FeedName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "單位")]
        public string UnitName { get; set; } = "公斤";

        [StringLength(100)]
        [Display(Name = "備註")]
        public string? Remark { get; set; }
    }
}
